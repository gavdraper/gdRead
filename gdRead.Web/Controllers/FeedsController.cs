using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using gdRead.Data;
using gdRead.Data.Models;
using gdRead.FeedUtils;
using Microsoft.AspNet.Identity;

namespace gdRead.Web.Controllers
{
    public class FeedsController : ApiController
    {
        [Authorize]
        public IEnumerable<Feed> Get()
        {

            var userId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
            var ctx = new gdReadContext();
            var feeds =
                (
                    from subscription in ctx.Subscriptions
                    join feed in ctx.Feeds on subscription.Feed.Id equals feed.Id into joinedFeeds
                    from jf in joinedFeeds.DefaultIfEmpty()
                    where subscription.UserId == userId
                    select jf
                    );

            return feeds;
        }

        public class FeedPostModel
        {
            public string Url { get; set; }
        }

        [Authorize]
        public void Post([FromBody] FeedPostModel feedPost)
        {
            var ctx = new gdReadContext();
            var feed = ctx.Feeds.FirstOrDefault(x => x.Url == feedPost.Url);
            if (feed == null)
            {
                //Find the actual RSS URL 
                feedPost.Url = FeedUrlFinder.FindFeedUrl(feedPost.Url);                
                feed = new Feed { Url = feedPost.Url};
                ctx.Feeds.Add(feed);
                ctx.SaveChanges();
                
                var subscription = new Subscription
                {
                    UserId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId()),
                    Feed = feed
                };
                ctx.Subscriptions.Add(subscription);
                ctx.SaveChanges();

                Fetcher.FetchFeed(feed.Id);
            }
            else
            {
                var userId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
                var subscription = ctx.Subscriptions.FirstOrDefault(x => x.UserId == userId &&
                                                                         x.Feed.Id == feed.Id);
                if (subscription != null) return;
                subscription = new Subscription
                {
                    UserId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId()),
                    Feed = feed
                };
                ctx.Subscriptions.Add(subscription);
                ctx.SaveChanges();
            }
            

        }
    }
}