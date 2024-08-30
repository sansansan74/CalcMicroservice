namespace LogWriter.Model;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class LogMessage
{
    [BsonId]
    public string Id { get; set; }

    [BsonElement("level")]
    public string Level { get; set; }
    
    [BsonElement("message")]
    public string MessageTemplate { get; set; }
    
    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; }

    [BsonElement("properties")]
    public Dictionary<string, object> Properties { get; set; }
}
