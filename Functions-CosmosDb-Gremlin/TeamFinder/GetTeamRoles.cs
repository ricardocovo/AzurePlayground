using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;

namespace TeamFinder
{
    /// <summary>
    /// Mock
    /// </summary>
    public static class GetTeamRoles
    {
        [FunctionName("GetTeamRoles")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "{customerName}/rolemappings")] HttpRequest req,
            string customerName, ILogger log)
        {
            log.LogInformation($"GetTeamRoles for {customerName}- Request Started");
            if (customerName.ToLower() != "contoso") return new NotFoundObjectResult($"No roles found for '{customerName}'");

            return new OkObjectResult(GetResponse());
        }

        public static object[] GetResponse()
        {
            return new object[]{
                new
                {
                    roleID = 64,
                    fullName = "Adam West",
                    role = "App Innovation CSA",
                    pinned = true
                },
                new{
                    roleID= 65,
                    fullName= "Adam North",
                    role= "CSAM",
                    pinned= false
                },
                new{
                    roleID= 66,
                    fullName= "Adam East",
                    role= "Security CSA",
                    pinned= false
                },
               new {
                    roleID= 71,
                    fullName= "David West",
                    role= "Infra Specialist",
                    pinned= true
                },
                new{
                    roleID= 72,
                    fullName= "David North",
                    role= "Account Executive",
                    pinned= false
                },
                new{
                    roleID= 73,
                    fullName= "David East",
                    role= "CSAM Dev",
                    pinned= false
                }

                };
        }
    }
}
