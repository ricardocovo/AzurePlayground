using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public static class DeleteListing
    {
        [FunctionName("DeleteListing")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "listing/{id}")] HttpRequest req,
            [Sql("DELETE FROM [dbo].[Listings] WHERE Id = @Id", "SqlConnectionString", "@Id={id}")] IEnumerable<Object> result,
            ILogger log)
        {
            return new OkResult();
        }
    }
}
