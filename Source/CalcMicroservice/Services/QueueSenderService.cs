using CalcMicroservice.Messages.Servicing;
using MassTransit;
using Operations;
using System;
using System.Threading.Tasks;

namespace CalcMicroservice.Services
{
    public class QueueSenderService(IBus bus)
    {
        private readonly IBus _bus = bus;

        public async Task SendMessage<T, M>(string queueName, M op, ConsumeContext<T> context) 
            where T : class 
            where M : class
        {
            var sendEndpoint = await _bus.GetSendEndpoint(new Uri(Operations.Utils.GetRabbitQueueUri(queueName)));
            await sendEndpoint.Send(op, context.CancellationToken);
        }

        public async Task PublishMessage<T>(ConsumeContext<T> context, ProcessingResultMessage op) where T : class
        {
            await _bus.Publish(op, context.CancellationToken);
        }
    }
}
