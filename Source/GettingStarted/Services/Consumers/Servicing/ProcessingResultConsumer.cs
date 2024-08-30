using CalcMicroservice.Messages.Servicing;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CalcMicroservice.Services.Consumers.Servicing;

public class ProcessingResultConsumer(ILogger<ProcessingResultConsumer> logger, IBus bus) : IConsumer<ProcessingResultMessage>
{
    readonly ILogger<ProcessingResultConsumer> _logger = logger;
    private readonly IBus _bus = bus;

    public async Task Consume(ConsumeContext<ProcessingResultMessage> context)
    {
        var m = context.Message;
        _logger.LogInformation($"Received ProcessingResultMessage: TraceId={m.TraceId}, Values={m.Value}, Error={m.Error}, ErrorSource={m.ErrorSource}");
    }

}