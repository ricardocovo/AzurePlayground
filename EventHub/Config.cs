using Newtonsoft.Json;

public class Config
{
    public string EventHubConnectionString { get; set; }
    public string EventHubName { get; set; }
    public string StorageAccountConnectionString { get; set; }
    public string ContainerName { get; set; }
    public string ConsumerGroup { get; set; }

    public Config(string connectionString, string eventHubName, string storageAccountConnectionString, string containerName, string consumerGroup) { 
        EventHubConnectionString = connectionString;
        EventHubName = eventHubName;
        StorageAccountConnectionString = storageAccountConnectionString;
        ContainerName = containerName;
        ConsumerGroup = consumerGroup;
    }

    public static Config? Get()
    {
        using (StreamReader r = new StreamReader("appsettings.json"))
        {
            string json = r.ReadToEnd();
            return JsonConvert.DeserializeObject<Config>(json);
        }
    }
}