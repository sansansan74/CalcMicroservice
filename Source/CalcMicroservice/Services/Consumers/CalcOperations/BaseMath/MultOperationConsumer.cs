using CalcMicroservice.Messages.Operations;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Operations;
using System.Collections.Generic;

namespace CalcMicroservice.Services.Consumers.CalcOperations.BaseMath;

public class MultOperationConsumer(ILogger<OperationConsumerBase> logger, IConfiguration configuration, QueueSenderService queueSender) :
    OperationConsumerBase(logger, configuration,  queueSender, OperationNames.Mult, MicroserviceNames.OperationMult), IConsumer<OperationConditionMessage>
{
    protected override double ProcessOperation(OperationConditionMessage m, List<double> argumentsList)
    {
        double sum = 1;
        foreach (var arg in argumentsList)
        {
            sum *= arg;
        }

        return sum;
    }

}