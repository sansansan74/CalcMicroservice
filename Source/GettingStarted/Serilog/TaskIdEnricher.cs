using Serilog.Core;
using Serilog.Events;
using System.Threading.Tasks;

namespace CalcMicroservice.Serilog;

public class TaskIdEnricher : ILogEventEnricher
{
    public const string TaskIdPropertyName = "TaskId";

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var taskId = Task.CurrentId.HasValue ? Task.CurrentId.Value : -1;
        logEvent.AddPropertyIfAbsent(new LogEventProperty(TaskIdPropertyName, new ScalarValue(taskId)));
    }
}
