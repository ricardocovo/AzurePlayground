using ExRam.Gremlinq.Core;
using Microsoft.Extensions.Logging;
using System;
using ExRam.Gremlinq.Core.Models;
using Gremlin.Net.Structure;
using ExRam.Gremlinq.Providers.WebSocket;
using System.Threading.Tasks;
using TeamFinder.Model;

namespace TeamFinder
{
    public static class Helper
    {
        private static string GremlinEndpoint => Environment.GetEnvironmentVariable("GremlinEndpoint") ?? throw new ArgumentException("Missing env var: GremlinEndpoint");
        private static string CosmosKey => Environment.GetEnvironmentVariable("CosmosKey") ?? throw new ArgumentException("Missing env var: CosmosKey");

        public static IGremlinQuerySource GetQuerySource(ILogger log) { 

            return GremlinQuerySource.g
               .ConfigureEnvironment(env => env.UseLogger(log))
               .ConfigureEnvironment(env => env
                   .UseModel(GraphModel
                       .FromBaseTypes<Vertex, Edge>(lookup => lookup
                           .IncludeAssembliesOfBaseTypes())
                       )
                   .ConfigureOptions(options => options
                       .SetValue(WebSocketGremlinqOptions.QueryLogLogLevel, LogLevel.None)))
                .UseCosmosDb(configurator => configurator
                    .At(new Uri(GremlinEndpoint))
                    .OnDatabase("CustomerRoleMappingsDb")
                    .OnGraph("customerRoleMapping")
                    .AuthenticateBy(CosmosKey));
        }

        public static async Task CreateGraph(IGremlinQuerySource querySource, ILogger log) {
            
            log.LogInformation("Clearing the database...");

            await querySource
                .V()
                .Drop();


            log.LogInformation("Creating a new database...");

            var tomnorth = await querySource
                .AddV(new Person { Name = "Tom North", Role = "Infra Specialist" })
                .FirstAsync();

            var tomeast = await querySource
                .AddV(new Person { Name = "Tom East", Role = "AppInnovation Specialist" })
                .FirstAsync();

            var adamnorth = await querySource
                .AddV(new Person { Name = "Adam North", Role = "Infra CSA" })
                .FirstAsync();

            var adameast = await querySource
                .AddV(new Person { Name = "Adam East", Role = "AppInnovation CSA" })
                .FirstAsync();

            var contoso = await querySource
                .AddV(new Customer { Name = "contoso", FullName = "Contoso Inc" })
                .FirstAsync();

            var bumbleBee = await querySource
                .AddV(new Customer { Name = "bumblebee", FullName = "BumbleBee Inc" })
                .FirstAsync();

            await querySource
                .V(contoso.Id!)
                .AddE<Has>()
                .To(__ => __
                    .V(adamnorth.Id!))
                .FirstAsync();

            await querySource
                .V(contoso.Id!)
                .AddE<Has>()
                .To(__ => __
                    .V(adameast.Id!))
                .FirstAsync();

            await querySource
                .V(contoso.Id!)
                .AddE<Has>()
                .To(__ => __
                    .V(tomeast.Id!))
                .FirstAsync();

            await querySource
                .V(bumbleBee.Id!)
                .AddE<Has>()
                .To(__ => __
                    .V(adamnorth.Id!))
                .FirstAsync();

            await querySource
                 .V(bumbleBee.Id!)
                 .AddE<Has>()
                 .To(__ => __
                     .V(tomeast.Id!))
                 .FirstAsync();


        }

    }
}
