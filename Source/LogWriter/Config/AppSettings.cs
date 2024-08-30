namespace LogWriter.Config;

public class AppSettings
{
    public RabbitMQSettings RabbitMQ { get; set; }
    public MongoDBSettings MongoDB { get; set; }
    public LogSaverSettings LogSaver { get; set; }
    
}
