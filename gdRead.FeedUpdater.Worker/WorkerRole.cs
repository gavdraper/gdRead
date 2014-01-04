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
using Microsoft.WindowsAzure.ServiceRuntime;

namespace gdRead.FeedUpdater.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly gdReadContext _ctx = new gdReadContext();

        private string getFeedUrl(string url)
        {
            if (url.EndsWith("feed/", StringComparison.CurrentCultureIgnoreCase) ||
                url.EndsWith("feed", StringComparison.CurrentCultureIgnoreCase))
                return url;
            if (!url.EndsWith("/"))
                url += "/";
            url += "feed/";
            return url;
        }

        private DateTime getLastPublishDate(Feed subscription)
        {
            var ctx = new gdReadContext();
            var lastPost = (from post in ctx.Posts
                            where post.Feed.Id == subscription.Id
                            orderby post.PublishDate descending
                            select post).FirstOrDefault();
            return lastPost == null ? DateTime.MinValue : lastPost.PublishDate;
        }

        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("gdRead.FeedUpdater.Worker entry point called");
            while (true)
            {
                foreach (var feed in _ctx.Feeds.ToList())
                {
                    SyndicationFeed rssFeed;
                    using (
                        var feedXml = XmlReader.Create(getFeedUrl(feed.Url),
                            new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse }))
                    {
                        rssFeed = SyndicationFeed.Load(feedXml);
                        feedXml.Close();
                    }

                    var lastPublishDate = getLastPublishDate(feed);

                    foreach (var feedPost in rssFeed.Items.Where(x => x.PublishDate.UtcDateTime > lastPublishDate))
                    {

                        var post = new Post()
                        {
                            Url = feedPost.Links.FirstOrDefault() == null ? "" : feedPost.Links.First().Uri.ToString(),
                            Name = feedPost.Title.Text,
                            Summary = feedPost.Summary.Text,

                            Feed = feed,
                            PublishDate = feedPost.PublishDate.UtcDateTime,
                            Read = false
                        };

                        //get the encoded content or content
                        if (feedPost.Content != null)
                            post.Content = post.Content.ToString();
                        else
                        {
                            foreach (var ext in feedPost.ElementExtensions)
                            {
                                var ele = ext.GetObject<XElement>();
                                if (ele.Name.LocalName == "encoded" && ele.Name.Namespace.ToString().Contains("content"))
                                {
                                    post.Content = ele.Value;
                                    break;
                                }
                            }
                        }

                        _ctx.Posts.Add(post);
                        _ctx.SaveChanges();
                    }
                }
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
