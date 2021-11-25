using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

public class ScheduleScenario : BaseScenario
{
    public ScheduleScenario(Config config) : base(config.ConnectionString, config.QueueName1) { }

    public override async Task Run()
    {
        //ssend a message
        var messageBody = new { body = "Schedule Scenario Message. {DateTime.Now.Ticks}", AttemptCount = 0 };
        await _sender.SendMessageAsync(new ServiceBusMessage(JsonConvert.SerializeObject(messageBody)));
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
                //sechedule
                var newMessage = new ServiceBusMessage(message.Body);
                await _sender.ScheduleMessageAsync(new ServiceBusMessage(message.Body), DateTimeOffset.Now.AddSeconds(3));
                //wait 
                Thread.Sleep(3000);

            }
            else
            {
                break;
            }
            count++;
        }
    }
}

