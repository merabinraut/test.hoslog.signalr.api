namespace hoslog.signalr.api.Models.Cache;
public class RedisCacheSetting //: IRedisCacheSetting
{
    public bool isEnabled { get; set; }
    public string connectionString { get; set; } = null!;
    public string instanceName {get;set;} = null!;
    public string channelPrefix { get; set; } = null!;
    public string password { get; set; } = null!;
    public int minimumIntervalInMinutes { get; set; }
    public int mediumIntervalInMinutes { get; set; }
    public int maximumIntervalInMinutes { get; set; }
    public int minimumIntervalInSeconds { get; set; }
    public int mediumIntervalInSeconds { get; set; }
    public int maximumIntervalInSeconds { get; set; }
}

// public interface IRedisCacheSetting
// {
//     bool isEnabled { get; }
//     string connectionString { get; }
//     string instanceName { get; }
//     string channelPrefix { get; }
//     string password { get; }
//     int minimumIntervalInMinutes { get; }
//     int mediumIntervalInMinutes { get; }
//     int maximumIntervalInMinutes{ get; }
//     int minimumIntervalInSeconds { get; }
//     int mediumIntervalInSeconds { get; }
//     int maximumIntervalInSeconds { get; }
// }