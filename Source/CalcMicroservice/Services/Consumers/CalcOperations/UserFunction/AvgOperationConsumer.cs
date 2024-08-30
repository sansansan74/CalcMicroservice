using CalcMicroservice.Messages.Operations;
using CalcMicroservice.Services.Consumers.CalcOperations.BaseMath;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Operations;
using System.Collections.Generic;
using System.Linq;

namespace CalcMicroservice.Services.Consumers.CalcOperations.UserFunction;

public class AvgOperationConsumer(ILogger<OperationConsumerBase> logger, IConfiguration configuration, QueueSenderService queueSender) :
    OperationConsumerBase(logger, configuration, queueSender, OperationNames.Avg, MicroserviceNames.OperationAdd), IConsumer<OperationConditionMessage>
{
    protected override double ProcessOperation(OperationConditionMessage m, List<double> argumentsList)
        => argumentsList.Average();
}