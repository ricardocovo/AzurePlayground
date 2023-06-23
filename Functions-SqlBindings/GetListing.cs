using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public static class GetListing
    {
        [FunctionName("GetListing")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "listing/{id}")] HttpRequest req,
            [Sql("SELECT * FROM [dbo].[Listings] WHERE Id = @Id", "SqlConnectionString", "@Id={id}")] IEnumerable<Object> result,
            ILogger log)
        {
            return result.Count() == 0 ?
                new NotFoundResult() :
                new OkObjectResult(result.First());
        }
    }
}
