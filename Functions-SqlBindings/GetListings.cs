using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public static class GetListings
    {
        [FunctionName("GetListings")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "listing")] HttpRequest req,
            [Sql("SELECT * FROM [dbo].[Listings] WHERE IsActive = 1", "SqlConnectionString")] IEnumerable<Object> result,
            ILogger log)
        {
            return new OkObjectResult(result);
        }
    }
}
