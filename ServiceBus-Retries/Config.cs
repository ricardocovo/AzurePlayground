using Newtonsoft.Json;

public class Config
{
    public string ConnectionString { get; set; }
    public string QueueName1 { get; set; }
    public string QueueName2 { get; set; }
    public Config(string connectionString, string queueName1, string queueName2 ) { 
        ConnectionString = connectionString;
        QueueName1 = queueName1;
        QueueName2 = queueName2;
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