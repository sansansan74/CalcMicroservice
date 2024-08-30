using CalcMicroservice.Messages.Operations;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Operations;
using System.Collections.Generic;

namespace CalcMicroservice.Services.Consumers.CalcOperations.BaseMath;

public class SubtractOperationConsumer(ILogger<OperationConsumerBase> logger, IConfiguration configuration, QueueSenderService queueSender) :
    OperationConsumerBase(logger, configuration, queueSender, OperationNames.Subtract, MicroserviceNames.OperationSubtract), IConsumer<OperationConditionMessage>
{
    protected override double ProcessOperation(OperationConditionMessage m, List<double> argumentsList)
    {
        double sum = argumentsList[0];
        for (int i = 1; i < argumentsList.Count; i++)
        {
            double arg = argumentsList[i];
            sum -= arg;
        }

        return sum;
    }

}