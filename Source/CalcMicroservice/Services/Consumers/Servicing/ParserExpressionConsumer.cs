using CalcMicroservice.Messages;
using CalcMicroservice.Messages.Servicing;
using FormulaParser;
using FormulaParser.Tree;
using FormulaParser.Utils;
using MassTransit;
using Microsoft.Extensions.Logging;
using Operations;
using System;
using System.Threading.Tasks;

namespace CalcMicroservice.Services.Consumers.Servicing;

public class ParserExpressionConsumer(ILogger<ParserExpressionConsumer> logger, IBus bus) : IConsumer<ParseMessage>
{
    readonly ILogger<ParserExpressionConsumer> _logger = logger;
    private readonly IBus _bus = bus;

    public async Task Consume(ConsumeContext<ParseMessage> context)
    {
        var mes = context.Message;
        _logger.LogInformation($"Received ParseMessage: {mes.MathExpression}");

        var (treeExpression, err) = ParseExpression(mes.MathExpression);

        if (err == null)
        {
            var calcMessage = new CalcMessage
            {
                TraceId = mes.TraceId,
                TreeExpression = treeExpression
            };

            _logger.LogInformation($"Sending CalcMessage: TraceId={calcMessage.TraceId}, TreeExpression={calcMessage.TreeExpression}");

            var sendEndpoint = await _bus.GetSendEndpoint(new Uri(Operations.Utils.GetRabbitQueueUri(QueueNames.CalcMessage)));
            await sendEndpoint.Send(calcMessage, context.CancellationToken);

            return;
        }

        // error occured
        var op = new ProcessingResultMessage
        {
            TraceId = mes.TraceId,
            Value = -1,
            Error = err,
            ErrorSource = MicroserviceNames.ServicingParser
        };

        _logger.LogInformation($"Publishing ProcessingResultMessage: TraceId={op.TraceId},  Value={op.Value}, Error={op.Error}, ErrorSource={op.ErrorSource}");
        await _bus.Publish(op, context.CancellationToken);
    }

    private (string res, string err) ParseExpression(string mathExpression)
    {
        var parser = new ExpressionParser();

        try
        {
            var tree = parser.Parse(mathExpression);
            TreeWrapper treeWrapper = new(null);
            treeWrapper.Value = tree;
            var json = ExpressionSerializer.SerializeToJson(treeWrapper);
            //string resultString = TreePrinter.Print(tree);
            return (res: json, err: null);
        }
        catch (Exception ex)
        {
            return (res: null, err: ex.Message);
        }
    }
}