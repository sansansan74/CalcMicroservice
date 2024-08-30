using CalcMicroservice.Messages.Operations;
using CalcMicroservice.Messages.Servicing;
using FormulaCalculator.Utils;
using FormulaParser.Tree;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CalcMicroservice.Services.Consumers.Servicing;

public class OperationResultsConsumer(ILogger<OperationResultsConsumer> logger, ITreePersistantStorageService treePersistantStorageService, CalculateExpressionService calculateExpressionService, QueueSenderService queueSenderService) : IConsumer<OperationResultsMessage>
{
    readonly ILogger<OperationResultsConsumer> _logger = logger;
    private readonly ITreePersistantStorageService _treePersistantStorageService = treePersistantStorageService;
    private readonly CalculateExpressionService _calculateExpressionService = calculateExpressionService;

    public async Task Consume(ConsumeContext<OperationResultsMessage> context)
    {
        var m = context.Message;
        _logger.LogInformation($"Received OperationResultsMessage: TraceId={m.TraceId}, OperationId={m.OperationId}, Values={m.Value}, Error={m.Error}");


        // Пытаемся закешировать результат вычисления
        // Есть 2 варианта:
        // Все вычисление сделано
        // Вычислена одна маленькая фукнция add, mult
        string expression = GetExpression(m);
        string val = string.IsNullOrEmpty(m.Error) ? m.Value.ToString() : CreateErrorMessage(m);
        await _treePersistantStorageService.Set(expression, val);

        // GetTree
        if (!string.IsNullOrEmpty(m.Error))
        {
            var op = new ProcessingResultMessage
            {
                TraceId = m.TraceId,
                Error = CreateErrorMessage(m),
                ErrorSource = m.ErrorSource,
                Value = -1,
            };

            _logger.LogInformation($"Publishing ProcessingResultMessage: TraceId={op.TraceId},  Value={op.Value}, Error={op.Error}, ErrorSource={op.ErrorSource}");
            await queueSenderService.PublishMessage(context, op);
            return;
        }

        var tree = CalcExpressionSerializer.DeserializeFromJson(m.Expression);

        ICalcTreeItem replacedTree = CalcTreeUtils.ReplaceEvaluatedOperationInTree(tree, m.OperationId, m.Value); // Replace evaluated node result
        await _calculateExpressionService.ProcessCalcTree(m.TraceId, context, replacedTree);
    }

    private static string CreateErrorMessage(OperationResultsMessage m)
    {
        return $"{m.Error}. Operation {m.OperationName}, Arguments: {m.Arguments}";
    }

    private string GetExpression(OperationResultsMessage m)
    {
        var itemsStr = (m.Arguments ?? string.Empty).Replace('[', '(').Replace(']', ')');
        return $"{m.OperationName}{itemsStr}";
    }
}