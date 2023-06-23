using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SqlFunctions.Models;

namespace Company.Function
{
    public static class UpdateListing
    {
        [FunctionName("UpdateListing")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "listing/{id}")] HttpRequest req,
            Guid id,
            ILogger log,
            [Sql("[dbo].[Listings]", "SqlConnectionString")] IAsyncCollector<Listing> listings)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var listing = JsonConvert.DeserializeObject<Listing>(requestBody);
            if (id != listing.Id)
            {
                return new BadRequestResult();

            }
            //prepare
            listing.UpdatedDate = DateTime.Now;

            await listings.AddAsync(listing);
            await listings.FlushAsync();

            return new OkObjectResult(listing);
        }
    }

}
