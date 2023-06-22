using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Sql;
using Microsoft.Extensions.Logging;
using SqlFunctions.Models;

namespace SqlFunctions
{
    public static class ChangeFeedTrigger
    {
    [FunctionName("ChangeFeedTrigger")]
        public static void Run(
            [SqlTrigger("[dbo].[Listings]", "SqlConnectionString")]
            IReadOnlyList<SqlChange<Listing>> changes,
            ILogger logger)
        {
            foreach (SqlChange<Listing> change in changes)
            {
                Listing listing = change.Item;
                logger.LogInformation($"Change operation: {change.Operation}");
                logger.LogInformation($"Id: {listing.Id}, Title: {listing.Name}");
            }
        }
    }
}
