gdReader
========

.Net open source azure based RSS reader

Trello board for development can be found at https://trello.com/b/bWAcRppU

The latest build can be seen running (for now) at http://gdread.azurewebsites.net

## Project Breakdown ##

 **gdRead.Data**

Contains the Entity Framework Code First models and context.

**gdRead.FeedUpdater**

This is the core Azure project used for deployment. You will need to change this project to point to your own Azure Subscription to self publish.

**gdRead.FeedUpdater.Worker** 

Azure Worker role. This contains the feed fetcher service that runs on Azure. This service runs on an interval and loops through all feeds in the DB to pull in any posts that have a publish date greater than the last publish date we already have.

**gdRead.Web**

This contains the website and the WebApi for gdRead.


