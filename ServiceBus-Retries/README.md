# Service Bus - Queue Retries
This repo explores diferent scenarios of retryability using Service Bus Queues. It is important to understand what comes out of the box and what you have to do in certain scenarios to ensure the messages are processed correctly or retried as your application requires.

## What you will need to run the scenarios
You will need a Service Bus Namespace. Once you have it:
* Rename `appsettings_template.json` to `appsettings.json`
* Open the file and add your connection string.

## Creating the Queues
In the `provisioning.azcli` file you will find scripts to create and delete the queues needed. (You can also create them manually if you preffer, but keep the configurations below).

Let's explore one of them:

``` powershell
az servicebus queue create `
    --resource-group {YourResourceGroup} `
    --namespace-name retry-tests `
    --name short-lock-duration `
    --enable-dead-lettering-on-message-expiration true `
    --lock-duration "PT5S" `
    --max-delivery-count 3 
```

We are creating a queue in named `short-lock-duration` with the following configuraiton:
 * We have enabled dead lettering, 
 * The lock-duration is 5 seconds 
 * The max delivery count is 3

[Docs: az servicebus queue create](https://docs.microsoft.com/en-us/cli/azure/servicebus/queue?view=azure-cli-latest#az_servicebus_queue_create)

## Scenarios
### Abandon a message
`dotnet run abandon`

In this scenario, we receive the message, and then abandon it, we do this 3 times in a row. 

Abandoning can happen when you decide that you cannot complete procesing the message. Abandoning a message is an explicit operation:
``` C#
var message = await _receiver.ReceiveMessageAsync();
...
await _receiver.AbandonMessageAsync(message);
```
Result:

* When abandoning
    * The message lock is lost.
    * The message becomes available again.
* The next time we receive the message, we get the exact same message, but the delivery count is increased by 1.
* Since we abandon the message 3 times, which is the max delivery count, the message is moved to the deadletter.
* When we get the message from the deadletter, we get the exact original message, with the delivery count increased by 1.


### Let a message expire
`dotnet run expire`

In this scenario, we will let the lock expire. In our `short-lock-duration` queue, the lock duration is 5 seconds,  so we wait in the code for 6 seconds to force the expiration.

Expiration of locks happens when your code takes longer than the queue lock duration, which can happen for any reason (external dependencies, etc). Setting a reasonable lock duration is important.

Lock expiration is an implicit operation.

Result:

* When the lock expires
    * The message lock is lost.
    * The message becomes available again.
* The next time we receive the message, we get the exact same message, but the delivery count is increased by 1.
* Since we let the message expire 3 times, which is the max delivery count, the message is moved to the deadletter.
* When we get the message from the deadletter, we get the exact original message, with the delivery count increased by 1.

### Deferring a message
`dotnet run defer`

In the deferring scenarion, we chose to defer procesing the message. It is basically putting the message on a "deferred-area" which is part of the regular queue infrastructure, until you are ready to process the message. You must save the message sequence number to be able to retreive it later.

Deferring is useful in workflow and other scenarios, where we are not ready to fully process the message until some dependecies occur.

Deferring is an explicit operation, you receiver a message, defer it, and then get it from the deferred queue. Here is a example:

``` C#
    var message = await _receiver.ReceiveMessageAsync();
    ...
    //Save the sequence to be able to retrive later
    var sequenceNumber = message.SequenceNumber;
    // defer message    
    await _receiver.DeferMessageAsync(message);
    // Receive from deadletter
    var deferredMessage = await _receiver.ReceiveDeferredMessageAsync(sequenceNumber);
    ...
```

Results:

* When deferring
    * The message is added to the deferred-area
    * The message will remain in the deferred-area until is explictly requested. 
* When we receive the deferred message, we get the exact same message, but the delivery count is increased by 1.


### Schedule a message
`dotnet run schedule`

Scheduling a message is its own functionality, however, it is very useful when thinking about your retry strategy. The Queue infrastructure has a separate queue on it dedicated to scheduled messages, when we schedule a message, it is put into that queue

In this scenarion we get a message and re-schedule it to be processed on a later time. This is useful, for example, if a dependency is temporarily down or if we want to defer processing the message without having to use the defer funcionality.

Scheduling a message is creating a new message, so you will basically get a new Id, sequence number and delivery,  etc. However, this method can still be useful since normally what you are interested on is inside the body of the message.

Here is how is working on this scenario we are scheduling the message to come back in 3 seconds in the same queue:

``` C#
    var message = await _receiver.ReceiveMessageAsync();    
    // Schedule Message
    var newMessage = new ServiceBusMessage(message.Body);
    await _sender.ScheduleMessageAsync(new ServiceBusMessage(message.Body), DateTimeOffset.Now.AddSeconds(3));
    // Complete message so it is removed form the active queue
    await _receiver.CompleteMessageAsync(message);
    //Wait    
    Thread.Sleep(3000);
    // receive again and complete
    message = await _receiver.ReceiveMessageAsync();
    await _receiver.CompleteMessageAsync(message);
```

Results:
* Since we completed the message, the original message is removed from the Queue.
* A new message is created into the Scheduled Queue.
* After 3 seconds, the message is moved from the scheduled Queue into the Active Queue.
* We receive the message again from the active queue, it is a new message:
    * It has a new Id, Sequence Number and Delivery Count.
    * It has the same body since we used it to create the new message
    * The message will remain in the deferred-area until is explictly requested. 

### Implementing Sliding Retry Schedule
`dotnet run sliding-schedule`

It is a common practice to retry a message in increasing, sliding intervals. For example:
* If the the first attempt fails, retry in 1 second
* if the second attempt fails, retry in 3 seconds
* if the second attempt fails, retry in 9 seconds,
* etc.

We can use the scheduling concept to implement a sliding retry schedule, however, since when we schedule a message we loose our Delivery Count, we will have to keep the `AttemptCount` in the body of the message.

In this scenario we implement the sliding retry schedule, we use the fibonacci sequence in conjuction to the attempt count and a constant to calculate how long we want to wait, but you could change the `CalculateDelay()` function with your own logic. 

Take a look at the scenario code, but the summary is as follow:

``` C#
    var message = await _receiver.ReceiveMessageAsync();
    //complete message so it is removed from the active queue
     await _receiver.CompleteMessageAsync(message);
    // "CustomMessageBody" contains the AttemptCount
    var receivedMessageBody = JsonConvert.DeserializeObject<CustomMessageBody>(message.Body.ToString());
    receivedMessageBody.AttemptCount++;
    var waitTime = CalculateDelay(receivedMessageBody.AttemptCount);
    // schedule message
    var newMessage = new ServiceBusMessage(JsonConvert.SerializeObject(receivedMessageBody));
                    await _sender.ScheduleMessageAsync(
                                new ServiceBusMessage(JsonConvert.SerializeObject(receivedMessageBody)),
                                DateTimeOffset.Now.AddSeconds(waitTime));
    // Complete message so it is removed form the active queue
    await _receiver.CompleteMessageAsync(message);
```
Results:
* Since we completed the message, the original message is removed from the Queue.
* A new message is created into the Scheduled Queue (similar to last scenario)
* We re-scheduled the message 4 times:
    * The first time is scheduled it for 1  seconds in the future
    * The second time is scheduled it for 6 seconds in the future
    * The second time is scheduled it for 9 seconds in the future
    * The second time is scheduled it for 15 seconds in the future
* We are enforcing a max number of attempts of 4, after that, we send to the dead letter

