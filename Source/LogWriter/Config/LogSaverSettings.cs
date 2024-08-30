namespace LogWriter.Config;

public class LogSaverSettings
{
    public Int32 InsertDbBatchSize { get; set; }
    public Int32 ReadFromRabbitCacheSize { get; set; }
    public Int32 DelayMilliseconds { get; set; }
}
