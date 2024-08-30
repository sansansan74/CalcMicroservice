using CalcMicroservice.Messages.Operations;
using CalcMicroservice.Services.Consumers.CalcOperations.BaseMath;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Operations;
using System;
using System.Collections.Generic;

namespace CalcMicroservice.Services.Consumers.CalcOperations.UserFunction;

public class AbsOperationConsumer(ILogger<OperationConsumerBase> logger, IConfiguration configuration, QueueSenderService queueSender) :
    OperationConsumerBase(logger, configuration, queueSender, OperationNames.Abs, MicroserviceNames.OperationAbs), IConsumer<OperationConditionMessage>
{
    protected override double ProcessOperation(OperationConditionMessage m, List<double> argumentsList)
    {
        CheckParamsCount(m, argumentsList, 1);
        return Math.Abs(argumentsList[0]);
    }

}