using Azure.Messaging.ServiceBus;

public abstract class BaseScenario
{
    internal ServiceBusClient _client;
    internal ServiceBusSender _sender;
    internal ServiceBusReceiver _receiver;
    internal ServiceBusReceiver _deadLetterReceiver;
    public BaseScenario(string connectionString, string queueName)
    {
        Console.WriteLine($"Queue: {queueName}");
        _client = new ServiceBusClient(connectionString);
        _sender = _client.CreateSender(queueName);
        _receiver = _client.CreateReceiver(queueName);
        _deadLetterReceiver = _client.CreateReceiver(queueName, new ServiceBusReceiverOptions() { SubQueue = SubQueue.DeadLetter });
    }

    public abstract Task Run();
}

