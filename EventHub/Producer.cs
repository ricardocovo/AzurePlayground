using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System.Text;

namespace EventHub
{
    internal class Producer
    {
        public static async Task PostEvents(Config config, int numberOfEvents) {
            // Create a producer client that you can use to send events to an event hub
            var producerClient = new EventHubProducerClient(config.EventHubConnectionString, config.EventHubName);

            // Create a batch of events 
            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

            for (int i = 1; i <= numberOfEvents; i++)
            {
                
                if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes($"Event {Guid.NewGuid()} - {DateTime.Now.ToString()}"))))
                {
                    // if it is too large for the batch
                    throw new Exception($"Event {i} is too large for the batch and cannot be sent.");
                }
            }

            try
            {
                // Use the producer client to send the batch of events to the event hub
                await producerClient.SendAsync(eventBatch);
                Console.WriteLine($"A batch of {numberOfEvents} events has been published.");
            }
            finally
            {
                await producerClient.DisposeAsync();
            }
        }
    }
}
