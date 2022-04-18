using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using System.Text;

namespace EventHub
{
    internal class Receiver
    {
        public static async Task Listen(Config config) {            
            var storageClient = new BlobContainerClient(config.StorageAccountConnectionString, config.ContainerName);
            var processor = new EventProcessorClient(storageClient, config.ConsumerGroup, config.EventHubConnectionString, config.EventHubName);

            // Register handlers for processing events and handling errors
            processor.ProcessEventAsync += ProcessEventHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;

            while (true)
            {
                // Start the processing
                await processor.StartProcessingAsync();

                // Wait for 30 seconds for the events to be processed
                await Task.Delay(TimeSpan.FromSeconds(30));

                // Stop the processing
                await processor.StopProcessingAsync();
            }
        }

        static async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            // Write the body of the event to the console window
            Console.WriteLine("\tReceived event: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));

            // Update checkpoint in the blob storage so that the app receives only new events the next time it's run
            await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
        }

        static Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            // Write details about the error to the console window
            Console.WriteLine($"\tPartition '{ eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
            Console.WriteLine(eventArgs.Exception.Message);
            return Task.CompletedTask;
        }
    }
}
