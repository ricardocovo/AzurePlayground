# Summary

This repo contains a sample of how to build an API using Azure Functions and SQL Bindings.

## Requirements for preview features

## Simple API

Look at the files:

- GetListings.cs (get)
- GetListing.cs (get/{id})
- CreateListing.cs (post)
- UpdateListing.cs (put/{id})
- Delete.cs (delete/{id})

# Change Feed

File: ChangeFeedTrigger.cs

To use the change feed trigger, you will need to use the latest verison of the Extension

```
dotnet add package Microsoft.Azure.WebJobs.Extensions.Sql --prerelease
```

and enable change tracking on your database

```
ALTER DATABASE [Database]
SET CHANGE_TRACKING = ON
(CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON);

ALTER TABLE [dbo].[Table]
ENABLE CHANGE_TRACKING;
```
