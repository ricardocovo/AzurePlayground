using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TeamFinder
{
    public static class SeedTeams
    {
        [FunctionName("SeedTeams")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {

            log.LogInformation($"SeedTeams  - Request Started");

            var querySource = Helper.GetQuerySource(log);
            await Helper.CreateGraph(querySource, log);

            return new OkObjectResult("Complete");
        }
    }
}
