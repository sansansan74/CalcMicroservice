using CalcMicroservice.Messages.Operations;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Operations;
using System;
using System.Collections.Generic;

namespace CalcMicroservice.Services.Consumers.CalcOperations.BaseMath;

public class DivideOperationConsumer(ILogger<OperationConsumerBase> logger, IConfiguration configuration, QueueSenderService queueSender) :
    OperationConsumerBase(logger, configuration, queueSender, OperationNames.Divide, MicroserviceNames.OperationDivide), IConsumer<OperationConditionMessage>
{
    protected override double ProcessOperation(OperationConditionMessage m, List<double> argumentsList)
    {
        double sum = argumentsList[0];
        for (int i = 1; i < argumentsList.Count; i++)
        {
            double arg = argumentsList[i];
            sum /= arg;
        }

        if (double.IsInfinity(sum) || double.IsNaN(sum))
        {
            throw new ArgumentException("You can not divide to 0");
        }

        return sum;
    }

}