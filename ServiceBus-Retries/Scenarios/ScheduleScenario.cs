using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

public class ScheduleScenario : BaseScenario
{
    public ScheduleScenario(Config config) : base(config.ConnectionString, config.QueueName1) { }

    public override async Task Run()
    {
        //ssend a message
        await _sender.SendMessageAsync(new ServiceBusMessage($"Schedule Scenario Message. {DateTime.Now.Ticks}"));
        Console.WriteLine("-> Message Sent");
        //receive and let expire
        var count = 0;
        Console.WriteLine($"\r\n-> Receiving... ");
        while (count < 2)
        {
            var message = await _receiver.ReceiveMessageAsync();
            if (message != null)
            {
                Console.WriteLine($"------ Delivery Count: {message.DeliveryCount} ------------");
                Console.WriteLine($"Message Enqueued Time: {message.EnqueuedTime}");
                Console.WriteLine($"Message Id: {message.MessageId}");
                Console.WriteLine($"Message Body: {message.Body}");
                await _receiver.CompleteMessageAsync(message);
                //Schedule                
                if (count < 1)
                {
                    var newMessage = new ServiceBusMessage(message.Body);
                    await _sender.ScheduleMessageAsync(new ServiceBusMessage(message.Body), DateTimeOffset.Now.AddSeconds(3));
                    Console.WriteLine("-> Message scheduled for 3 seconds from now");
                    //wait                 
                    Thread.Sleep(3000);
                }

            }
            else
            {
                break;
            }
            count++;
        }
    }
}

