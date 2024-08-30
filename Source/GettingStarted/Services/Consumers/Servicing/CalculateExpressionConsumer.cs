using CalcMicroservice.Messages.Servicing;
using FormulaCalculator.Utils;
using FormulaParser.Utils;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CalcMicroservice.Services.Consumers.Servicing;

public class CalculateExpressionConsumer(ILogger<CalculateExpressionConsumer> logger, CalculateExpressionService calculateExpressionService, ITreePersistantStorageService treePersistantStorageService) :
    IConsumer<CalcMessage>
{
    private readonly ILogger<CalculateExpressionConsumer> _logger = logger;
    private readonly CalculateExpressionService _calculateExpressionService = calculateExpressionService;
    private readonly ITreePersistantStorageService _treePersistantStorageService = treePersistantStorageService;
    public async Task Consume(ConsumeContext<CalcMessage> context)
    {
        var mes = context.Message;
        _logger.LogInformation($"Received CalcMessage: TraceId={mes.TraceId}, Tree={mes.TreeExpression}");

        await ProcessCalculation(mes, mes.TraceId.ToString(), context);
    }

    private async Task ProcessCalculation(CalcMessage mes, string traceId, ConsumeContext<CalcMessage> context)
    {
        // create tree with operation guids (operatinId)
        var wrapper = ExpressionSerializer.DeserializeFromJson(mes.TreeExpression);
        var calcTree = CalcTreeUtils.CreateTree(wrapper.Value);

        await _calculateExpressionService.ProcessCalcTree(traceId, context, calcTree);
    }
}