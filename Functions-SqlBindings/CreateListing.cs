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
    public static class CreateListing
    {
        [FunctionName("CreateListing")]
        public static async Task<CreatedResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "listing")] HttpRequest req,
            ILogger log,
            [Sql("[dbo].[Listings]", "SqlConnectionString")] IAsyncCollector<Listing> listings)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var listing = JsonConvert.DeserializeObject<Listing>(requestBody);
            listing.Id = Guid.NewGuid();
            listing.CreatedDate = listing.UpdatedDate = DateTime.Now;
            listing.IsActive = true;

            await listings.AddAsync(listing);
            await listings.FlushAsync();

            return new CreatedResult($"/listing", listing);
        }
    }

}
