using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

public struct CustomMessageBody
{
    public string Body { get; set; }
    public int AttemptCount { get; set; }
}

public class SlidingScheduleScenario : BaseScenario
{
    private int maxAttempts = 4;
    private int baseWaitTime = 3;
    private int[] fibonacci = new int[] { 1, 2, 3, 5, 8 };
    public SlidingScheduleScenario(Config config) : base(config.ConnectionString, config.QueueName2) { }

    public override async Task Run()
    {
        //ssend a message
        var messageBody = new { Body = $"Schedule Scenario Message. {DateTime.Now.Ticks}", AttemptCount = 0 };
        await _sender.SendMessageAsync(new ServiceBusMessage(JsonConvert.SerializeObject(messageBody)));
        Console.WriteLine("-> Message Sent");
        //receive and let expire
        var count = 0;
        Console.WriteLine($"\r\n-> Receiving... ");
        while (count < maxAttempts + 1)
        {
            var message = await _receiver.ReceiveMessageAsync();
            if (message != null)
            {
                Console.WriteLine($"------ Delivery Count: {message.DeliveryCount} ------------");
                Console.WriteLine($"Message Enqueued Time: {message.EnqueuedTime}");
                Console.WriteLine($"Message Id: {message.MessageId}");
                Console.WriteLine($"Message Body: {message.Body}");

                //sechedule
                var receivedMessageBody = JsonConvert.DeserializeObject<CustomMessageBody>(message.Body.ToString());
                if (receivedMessageBody.AttemptCount < maxAttempts)
                {
                    await _receiver.CompleteMessageAsync(message);
                    var waitTime = CalculateDelay(receivedMessageBody.AttemptCount);
                    receivedMessageBody.AttemptCount++;
                    Console.WriteLine($"-> Re-scheduling message...");
                    var newMessage = new ServiceBusMessage(JsonConvert.SerializeObject(receivedMessageBody));
                    await _sender.ScheduleMessageAsync(
                                new ServiceBusMessage(JsonConvert.SerializeObject(receivedMessageBody)),
                                DateTimeOffset.Now.AddSeconds(waitTime));
                    //wait 
                    Console.WriteLine($"Waiting {waitTime} seconds for attempt {receivedMessageBody.AttemptCount}");
                    Thread.Sleep(waitTime);
                }
                else
                {
                    Console.WriteLine($"-> Too many attempts, sending message to Dead-Lettered...");
                    await _receiver.DeadLetterMessageAsync(message);
                    break;
                }
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

    private int CalculateDelay(int attempt)
    {
        return fibonacci[attempt] * baseWaitTime;
    }
}

