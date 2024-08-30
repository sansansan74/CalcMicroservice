using LogWriter.Config;
using LogWriter.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace LogWriter.Services;
public class LogProcessorService : BackgroundService
{
    private readonly MongoDbSaverService _mongoDbSaverService;
    
    private readonly IConnection _rabbitConnection;
    private IModel _channel;
    private readonly string _queueName;

    public LogProcessorService(IOptions<AppSettings> settings, IConnection rabbitConnection,
                               MongoDbSaverService mongoDbSaverService)
    {
        var rabbitSettings = settings.Value.RabbitMQ;

        _rabbitConnection = rabbitConnection;
        _queueName = rabbitSettings.QueueName;
        _mongoDbSaverService = mongoDbSaverService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = _rabbitConnection.CreateModel();
        _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            LogMessage logMessage = CreateLogMessage(message);

            await _mongoDbSaverService.EnqueueMessage(logMessage);
  
        };

        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    private static LogMessage CreateLogMessage(string message)
    {
        LogMessage logMessage = JsonConvert.DeserializeObject<LogMessage>(message);
        logMessage.Id = Guid.NewGuid().ToString();

        if (logMessage.Properties.TryGetValue("EventId", out var _))
            logMessage.Properties.Remove("EventId");

        return logMessage;
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await _mongoDbSaverService.StopProcessingAsync();
        await base.StopAsync(stoppingToken);
    }

    public override void Dispose()
    {
        _channel.Dispose();
        _rabbitConnection.Dispose();
        base.Dispose();
    }
}
