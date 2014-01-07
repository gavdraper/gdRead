using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using gdRead.Data;
using gdRead.Data.Models;
using gdRead.FeedUtils;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace gdRead.FeedUpdater.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("gdRead.FeedUpdater.Worker entry point called");
            while (true)
            {
             //   Fetcher.FetchAllFeeds();
                Thread.Sleep(14400000);
            }
        }


        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
