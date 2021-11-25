using Azure.Messaging.ServiceBus;

public class ExpireScenario: BaseScenario
{
    public ExpireScenario(Config config) : base(config.ConnectionString, config.QueueName1) { }
    
    public override async Task Run()
    {
        //ssend a message
        await _sender.SendMessageAsync(new ServiceBusMessage($"Expire Scenario Message. {DateTime.Now.Ticks}"));
        Console.WriteLine("-> Message Sent");
        //receive and let expire         
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
                Console.WriteLine($"Sleeping for 6 seconds, lock duration is only 5 seconds...");
                Thread.Sleep(6000);

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

