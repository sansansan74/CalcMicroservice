using CalcMicroservice.Messages.Operations;
using CalcMicroservice.Messages.Servicing;
using FormulaCalculator.Utils;
using FormulaParser.Tree;
using MassTransit;
using Microsoft.Extensions.Logging;
using Operations;
using System.Linq;
using System.Threading.Tasks;

namespace CalcMicroservice.Services
{
    public class CalculateExpressionService(ILogger<CalculateExpressionService> logger, QueueSenderService queueSender, ITreePersistantStorageService treePersistantStorageService)
    {
        private readonly ILogger<CalculateExpressionService> _logger = logger;
        private readonly ITreePersistantStorageService _treePersistantStorageService = treePersistantStorageService;
        private readonly QueueSenderService _queueSender = queueSender;
        
        
        async Task<(CalcTreeOperation treeOp, CalcTreeOperation treeFull, ProcessingResultMessage repOp)> GetNextOperationForCalculateFromTree(string traceId, ICalcTreeItem calcTreeItem) {
            /*
             * Функция возвращает 2 значения:
             *      Node for publish (if calculation finished - end of tree of error in cache found)
             *      Node for send (we found node for calculation)
             CYCLE:
                  if tree is simple leaf, then send Publish with calculation result for finish and return null
                  get next complex operation from tree

                  try find this operation in cache
                  if item not found in cache, then return node (we found item for calculation!!!)

                  if get error from cache then throw Exception (was saved error result of previous calculation)

                  if get value from cache then Replace value in tree
            */
            ICalcTreeItem calcTree = calcTreeItem;

            while (true)
            {
                if (calcTree is CalcTreeLeaf leaf)
                {
                    // the tree is simple value - send response and finish
                    return (null, null, new ProcessingResultMessage
                    {
                        TraceId = traceId,
                        Error = null,
                        ErrorSource = null,
                        Value = leaf.Value
                    });
                }

                var calcTreeOperation = (CalcTreeOperation)calcTree;

                var operationForCalculate = CalcTreeUtils.FindCommandForCalculate(calcTreeOperation);
                
                var value = await _treePersistantStorageService.Get(operationForCalculate);
                if (value is null)  // complex node for calculate, no in cache
                    return (operationForCalculate, calcTreeOperation, null);


                var args = string.Join(",", operationForCalculate.Items);

                // this is evaluating error
                _logger.LogInformation($"Get value from cache TraceId={traceId}, Value={operationForCalculate.Operation}, Arguments={args}, Value={value}");

                if (!double.TryParse(value, out var doubleValue))
                {
                    return (null, null, new ProcessingResultMessage
                    {
                        TraceId = traceId,
                        Error = value,
                        ErrorSource = "From cache",
                        Value = -1
                    });
                }

                calcTree = CalcTreeUtils.ReplaceEvaluatedOperationInTree(calcTreeOperation, operationForCalculate.OperationId, doubleValue);
            }
        }

        public async Task ProcessCalcTree<T>(string traceId, ConsumeContext<T> context, ICalcTreeItem calcTree) where T : class
        {
            var result = await GetNextOperationForCalculateFromTree(traceId, calcTree);

            if (result.treeOp is not null)
            {
                var argList = result.treeOp.Items.Select(x => (x as CalcTreeLeaf).Value).ToList();
                var outMessage = new OperationConditionMessage
                {
                    TraceId = traceId,
                    OperationId = result.treeOp.OperationId,
                    OperationName = result.treeOp.Operation,
                    Arguments = System.Text.Json.JsonSerializer.Serialize(argList),
                    Expression = CalcExpressionSerializer.SerializeToJson(result.treeFull)
                };

                string queueName = Operations.Utils.GetQueueByOperation(result.treeOp.Operation);
                _logger.LogInformation($"Sending OperationConditionMessage: QueueName={queueName} TraceId={traceId}" +
                    $", OperationId={result.treeOp.OperationId}" +
                    $", OperationName={outMessage.OperationName}" +
                    $", Arguments={outMessage.Arguments}");

                await _queueSender.SendMessage(queueName, outMessage, context);

                return;
            }

            await Publish(context, result.repOp);
        }

        private async Task Publish<T>(ConsumeContext<T> context, ProcessingResultMessage opResult) where T : class
        {
            _logger.LogInformation($"Publishing ProcessingResultMessage: TraceId={opResult.TraceId},  Value={opResult.Value}, Error={opResult.Error}, ErrorSource={opResult.ErrorSource}");
            await _queueSender.PublishMessage(context, opResult);
        }

           
    }
}