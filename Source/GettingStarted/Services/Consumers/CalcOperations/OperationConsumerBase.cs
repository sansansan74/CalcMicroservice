using CalcMicroservice.Messages.Operations;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Operations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalcMicroservice.Services.Consumers.CalcOperations.BaseMath
{
    public abstract class OperationConsumerBase(ILogger<OperationConsumerBase> logger, IConfiguration configuration, QueueSenderService queueSender, string operationName, string microserviceName)
    {
        private readonly ILogger<OperationConsumerBase> _logger = logger;
        private readonly QueueSenderService _queueSender = queueSender;
        private readonly string _operationName = operationName;
        private readonly string _microserviceName = microserviceName;
        private readonly int _delay_milliseconds = configuration.GetValue<int>("Operations:delay_milliseconds", 0);

        public async Task Consume(ConsumeContext<OperationConditionMessage> context)
        {
            // Искусственно тормозим, чтобы можно было увидеть параллельную обработку разных запросов
            if (_delay_milliseconds != 0) 
                await Task.Delay(_delay_milliseconds);

            var m = context.Message;
            _logger.LogInformation($"Received: TraceId={m.TraceId}, OperationId={m.OperationId}, OperationName={m.OperationName}, Args={m.Arguments}, MicroserviceName={_microserviceName}");

            OperationResultsMessage op = null;

            try
            {
                if (m.OperationName != _operationName)
                    throw new Exception($"Operation consumer can process only {_operationName} operation. He get {m.OperationName} operation. MicroserviceName={_microserviceName}");

                List<double> argumentsList = string.IsNullOrEmpty(m.Arguments) ? new() : System.Text.Json.JsonSerializer.Deserialize<List<double>>(m.Arguments);
                double result = ProcessOperation(m, argumentsList);

                op = new OperationResultsMessage
                {
                    TraceId = m.TraceId,
                    OperationId = m.OperationId,
                    Value = result,
                    Error = null,
                    ErrorSource = null,
                    Expression = m.Expression,
                    OperationName = m.OperationName,
                    MicroserviceName = _microserviceName,
                    Arguments = m.Arguments
                };
            }
            catch (Exception ex)
            {
                op = new OperationResultsMessage
                {
                    TraceId = m.TraceId,
                    OperationId = m.OperationId,
                    Value = -1,
                    Error = ex.Message,
                    Expression = m.Expression,
                    OperationName = m.OperationName,
                    Arguments = m.Arguments,
                    ErrorSource = m.OperationName,
                };
            }

            _logger.LogInformation($"Sending OperationResultsMessage: TraceId={op.TraceId},  OperationId={op.OperationId}, MicroserviceName={_microserviceName}, Value={op.Value}, Error={op.Error}");
            await _queueSender.SendMessage(QueueNames.ResultOperationMessage, op, context);
        }

        protected virtual double ProcessOperation(OperationConditionMessage m, List<double> argumentsList)
        {
            throw new NotImplementedException();
        }

        protected static void CheckParamsCount(OperationConditionMessage m, List<double> argumentsList, int mustHaveParamsCount)
        {
            if (argumentsList.Count != mustHaveParamsCount)
            {
                throw new ArgumentException($"Operation '{m.OperationName}' must have 1 parameter. You call '{m.OperationName}' with {argumentsList.Count} parameters.");
            }
        }
    }
}