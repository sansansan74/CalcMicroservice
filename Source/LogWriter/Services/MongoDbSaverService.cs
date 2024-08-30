using LogWriter.Config;
using LogWriter.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Concurrent;

namespace LogWriter.Services;
public class MongoDbSaverService
{
    private readonly IMongoCollection<LogMessage> _collection;
    private readonly ConcurrentQueue<LogMessage> _messageQueue;
    private readonly CancellationTokenSource _cts;
    private readonly Task _processingTask;
    private readonly LogSaverSettings _logSaverSettings;

    public MongoDbSaverService(IMongoClient mongoClient, IOptions<AppSettings> settings)
    {
        _logSaverSettings = settings.Value.LogSaver;
        var mongoSettings = settings.Value.MongoDB;

        _collection = mongoClient
            .GetDatabase(mongoSettings.DatabaseName)
            .GetCollection<LogMessage>(mongoSettings.CollectionName);

        _messageQueue = new();
        _cts = new CancellationTokenSource();
        _processingTask = Task.Run(() => ProcessBatch(_cts.Token));
    }

    public async Task EnqueueMessage(LogMessage message)
    {
        if (_messageQueue.TryGetNonEnumeratedCount(out var count))
        {
            if (count > _logSaverSettings.ReadFromRabbitCacheSize)
            {
                await Task.Delay(_logSaverSettings.DelayMilliseconds);
            }
        }
        _messageQueue.Enqueue(message);
    }

    public async Task StopProcessingAsync()
    {
        await ProcessRemainingMessages();

        _cts.Cancel();
        await _processingTask;
    }

    private async Task ProcessBatch(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_messageQueue.IsEmpty)
            {
                try
                {
                    await Task.Delay(_logSaverSettings.DelayMilliseconds, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                
                continue;
            }

            List<LogMessage> messages = ReadMessagesFromCache();

            await WriteMessagesToMongoDb(messages, cancellationToken);
        }
    }

    private async Task ProcessRemainingMessages()
    {
        while (!_messageQueue.IsEmpty)
        {
            List<LogMessage> messages = ReadMessagesFromCache();
            await WriteMessagesToMongoDb(messages, CancellationToken.None);
        }
    }

    private async Task WriteMessagesToMongoDb(List<LogMessage> messages, CancellationToken cancellationToken)
    {
        if (messages.Count > 0)
        {
            var bulkOps = messages.Select(msg => new InsertOneModel<LogMessage>(msg))
                .Cast<WriteModel<LogMessage>>()
                .ToList();

            await _collection.BulkWriteAsync(bulkOps, cancellationToken: cancellationToken);
        }
    }

    private List<LogMessage> ReadMessagesFromCache()
    {
        var messages = new List<LogMessage>();

        while (messages.Count < _logSaverSettings.InsertDbBatchSize && _messageQueue.TryDequeue(out var message))
        {
            messages.Add(message);
        }

        return messages;
    }
}