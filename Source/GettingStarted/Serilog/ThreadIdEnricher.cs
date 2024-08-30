using Serilog.Core;
using Serilog.Events;
using System.Threading;

namespace CalcMicroservice.Serilog;

public class ThreadIdEnricher : ILogEventEnricher
{
    public const string ThreadIdPropertyName = "ThreadId";

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var threadId = Thread.CurrentThread.ManagedThreadId;
        logEvent.AddPropertyIfAbsent(new LogEventProperty(ThreadIdPropertyName, new ScalarValue(threadId)));
    }
}
