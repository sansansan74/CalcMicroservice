using CalcMicroservice.Messages.Servicing;
using MassTransit;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CalcMicroservice.Services.Workers
{
    public class ConsoleReaderWorker : BackgroundService
    {
        readonly IBus _bus;

        public ConsoleReaderWorker(IBus bus)
        {
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
                Console.WriteLine("Write math expression like 2+2 and push ENTER");
                string mathExpression = Console.ReadLine();

                await _bus.Publish(
                    new ParseMessage
                    {
                        MathExpression = mathExpression,
                        TraceId = Guid.NewGuid().ToString(),
                    }
                );
            }
        }
    }
}