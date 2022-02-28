using Azure.Messaging.ServiceBus;

public class NormalScenario : BaseScenario
{
    public NormalScenario(Config config) : base(config.ConnectionString, config.QueueName1) { }

    public override async Task Run()
    {
        //ssend a message
        await _sender.SendMessageAsync(new ServiceBusMessage($"Normal Scenario Message. {DateTime.Now.Ticks}"));
        Console.WriteLine("-> Message Sent");
        //receive and abandon
        Console.WriteLine($"\r\n-> Receiving... ");
        var message = await _receiver.ReceiveMessageAsync();
        if (message != null)
        {
            Console.WriteLine($"------ Delivery Count: {message.DeliveryCount} ------------");
            Console.WriteLine($"Message Id: {message.MessageId}");
            Console.WriteLine($"Message Body: {message.Body}");
            await _receiver.CompleteMessageAsync(message);
            Console.WriteLine($"Process Complete");
        }
    }
}

