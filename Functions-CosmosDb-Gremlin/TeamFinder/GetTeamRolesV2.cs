using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ExRam.Gremlinq.Core;
using ExRam.Gremlinq.Core.Models;
using Gremlin.Net.Structure;
using ExRam.Gremlinq.Providers.WebSocket;
using System.Security.Cryptography.X509Certificates;
using TeamFinder.Model;

namespace TeamFinder
{
    /// <summary>
    /// Connects to cosmos and returns result
    /// </summary>
    public static class GetTeamRolesV2
    {

        [FunctionName("GetTeamRolesV2")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "v2/{customerName}/rolemappings")] HttpRequest req,
            string customerName, ILogger log)
        {
            log.LogInformation($"GetTeamRolesV2 for {customerName} - Request Started");
            var querySource = Helper.GetQuerySource(log);

            var people = await querySource.V<Customer>()
                .Where(x => x.Name == customerName)
                .Out<Has>()
                .OfType<Person>().ToArrayAsync();
                
            if (people == null) return new NotFoundResult();

            return new OkObjectResult(people);
        }
    }
}
