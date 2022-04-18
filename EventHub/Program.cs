// See https://aka.ms/new-console-template for more information
if (args.Length == 0)
{
    Console.WriteLine($"Please enter a mode (producer/receiver)");
    return;
}

var config = Config.Get();
var mode = args[0];
var numOfEvents = 50;

if (config== null) {
    Console.WriteLine($"Config is null");
    return;
}


Console.WriteLine($"Starting {mode}");

if (mode == "producer")
{
    await EventHub.Producer.PostEvents(config, numOfEvents);
}
else {
    await EventHub.Receiver.Listen(config);
}