// See https://aka.ms/new-console-template for more information
//load config
var config = Config.Get();
if (config == null)
{
    Console.WriteLine("No Config");
    Environment.Exit(1);
}

// execute scenarios
var scenario = args.Length == 0 ? "" : args[0];
switch (scenario.ToLower())
{
    case "abandon":
        Console.WriteLine("Running the Abandon scenario");
        new AbandonScenario(config).Run().GetAwaiter().GetResult();
        break;
    case "expire":
        Console.WriteLine("Running the Expire scenario");
        new ExpireScenario(config).Run().GetAwaiter().GetResult();
        break;
    case "unhandled-exception":
        Console.WriteLine("Running the Unhandled Exception scenario");
        new UnhandledExceptionScenario(config).Run().GetAwaiter().GetResult();
        break;
    case "defer":
        Console.WriteLine("Running the Defer scenario");
        new DeferScenario(config).Run().GetAwaiter().GetResult();
        break;
    case "schedule":
        Console.WriteLine("Running the Schedule scenario");
        new ScheduleScenario(config).Run().GetAwaiter().GetResult();
        break;
    case "sliding-schedule":
        Console.WriteLine("Running the Sliding Schedule scenario");
        new SlidingScheduleScenario(config).Run().GetAwaiter().GetResult();
        break;
    default:
        Console.WriteLine("No scenario selected");
        Environment.Exit(1);
        break;
}
Environment.Exit(0);








