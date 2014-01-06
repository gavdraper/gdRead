using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using gdRead.Data;
using gdRead.Data.Models;

namespace gdRead.FeedUtils
{
    public class Fetcher
    {
        private static DateTime getLastPublishDate(Feed subscription)
        {
            var ctx = new gdReadContext();
            var lastPost = (from post in ctx.Posts
                            where post.Feed.Id == subscription.Id
                            orderby post.PublishDate descending
                            select post).FirstOrDefault();
            return lastPost == null ? DateTime.MinValue : lastPost.PublishDate;
        }


        public static void FetchFeed(int feedId)
        {
            var ctx = new gdReadContext();
            var attachedFeed = ctx.Feeds.FirstOrDefault(x => x.Id == feedId);
            SyndicationFeed rssFeed;
            using (
                var feedXml = XmlReader.Create(FeedUrlFinder.FindFeedUrl(attachedFeed.Url),
                    new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse }))
            {
                rssFeed = SyndicationFeed.Load(feedXml);
                feedXml.Close();
            }

            attachedFeed.Title = rssFeed.Title != null ? rssFeed.Title.Text : "No Title";

            var lastPublishDate = getLastPublishDate(attachedFeed);

            foreach (var feedPost in rssFeed.Items.Where(x => x.PublishDate.UtcDateTime > lastPublishDate))
            {

                var post = new Post()
                {
                    Url = feedPost.Links.FirstOrDefault() == null ? "" : feedPost.Links.First().Uri.ToString(),
                    Name = feedPost.Title.Text,
                    Summary = feedPost.Summary.Text,

                    Feed = attachedFeed,
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

                ctx.Posts.Add(post);
                ctx.SaveChanges();
            }
        }

        public static void FetchAllFeeds()
        {
            var ctx = new gdReadContext();
            foreach (var feed in ctx.Feeds.ToList())
            {
              FetchFeed(feed.Id);
            }
        }
    }
}
