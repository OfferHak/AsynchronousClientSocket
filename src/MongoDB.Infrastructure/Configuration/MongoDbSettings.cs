namespace MongoDB.Infrastructure.Configuration;

public class MongoDbSettings
{
    public const string SectionName = "MongoDbSettings";
    
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public int ConnectionTimeoutSeconds { get; set; } = 30;
    public int SocketTimeoutSeconds { get; set; } = 30;
    public bool EnableChangeStreams { get; set; } = true;
}