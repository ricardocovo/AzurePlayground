using Azure.Messaging.ServiceBus;

public class DeferScenario : BaseScenario
{
    public DeferScenario(Config config) : base(config.ConnectionString, config.QueueName1) { }

    public override async Task Run()
    {
        //ssend a message
        await _sender.SendMessageAsync(new ServiceBusMessage($"Defer Scenario Message. {DateTime.Now.Ticks}"));
        Console.WriteLine("-> Message Sent");
        //receive and defer
        Console.WriteLine($"\r\n-> Receiving... ");
        var message = await _receiver.ReceiveMessageAsync();
        Console.WriteLine($"------ Delivery Count: {message.DeliveryCount} ------------");
        Console.WriteLine($"Message Id: {message.MessageId}");
        Console.WriteLine($"Message Body: {message.Body}");
        Console.WriteLine($"Message SequenceNumber: {message.SequenceNumber}");
        var sequenceNumber = message.SequenceNumber;
        // defer message
        Console.WriteLine($"\r\n-> Defer message... ");
        await _receiver.DeferMessageAsync(message);

        // Receive from deadletter
        Console.WriteLine($"\r\n-> Receiving deferred message... ");
        var deferredMessage = await _receiver.ReceiveDeferredMessageAsync(sequenceNumber);
        Console.WriteLine($"Message Delivery Count: {deferredMessage.DeliveryCount}");
        Console.WriteLine($"Message Id: {deferredMessage.MessageId}");
        Console.WriteLine($"Message Body: {deferredMessage.Body}");
        await _receiver.CompleteMessageAsync(deferredMessage);
        Console.WriteLine($"-> Done!");
    }
}

