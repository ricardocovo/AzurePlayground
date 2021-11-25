using Azure.Messaging.ServiceBus;

public class AbandonScenario: BaseScenario
{
    public AbandonScenario(Config config) : base(config.ConnectionString, config.QueueName1) { }
    
    public override async Task Run()
    {
        //ssend a message
        await _sender.SendMessageAsync(new ServiceBusMessage($"Abandon Scenario Message. {DateTime.Now.Ticks}"));
        Console.WriteLine("-> Message Sent");
        //receive and abandon         
        var count = 0;
        Console.WriteLine($"\r\n-> Receiving... ");
        while (count < 3)
        {            
            var message = await _receiver.ReceiveMessageAsync();
            if (message != null)
            {
                Console.WriteLine($"------ Delivery Count: {message.DeliveryCount} ------------");
                Console.WriteLine($"Message Id: {message.MessageId}");
                Console.WriteLine($"Message Body: {message.Body}");
                await _receiver.AbandonMessageAsync(message);
            }
            else
            {
                break;
            }
            count++;
        }
        // Receive from deadletter
        Console.WriteLine($"\r\n-> Receiving from Dead Letter... ");
        var deadLetterMessage = await _deadLetterReceiver.ReceiveMessageAsync();
        Console.WriteLine($"Message Delivery Count: {deadLetterMessage.DeliveryCount}");
        Console.WriteLine($"Message Id: {deadLetterMessage.MessageId}");
        Console.WriteLine($"Message Body: {deadLetterMessage.Body}");
        await _deadLetterReceiver.CompleteMessageAsync(deadLetterMessage);
        Console.WriteLine($"-> Done!");
    }
}

