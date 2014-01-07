using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Configuration;
using gdRead.Data.Models;
using gdRead.Data.Repositories;
using gdRead.FeedUtils;
using Microsoft.AspNet.Identity;

namespace gdRead.Web.Controllers
{
    public class FeedsController : ApiController
    {
        private readonly string _conStr = ConfigurationManager.ConnectionStrings["gdRead.Data.gdReadContext"].ConnectionString;

        //Get all sub'ed feeds
        [Authorize]
        public IEnumerable<Feed> Get()
        {

            var userId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
            var feedRepository = new FeedRepository(_conStr);
            feedRepository.GetFeedById(57);
            return feedRepository.GetSubscribedFeeds(userId);
        }
        
        public class FeedPostModel
        {
            public string Url { get; set; }
        }

        //Add/Sub a feed
        [Authorize]
        public void Post([FromBody] FeedPostModel feedPost)
        {
            var feedRepository = new FeedRepository(_conStr);
            var subscriptionRepository = new SubscriptionRepository(_conStr);

            feedPost.Url = FeedUrlFinder.FindFeedUrl(feedPost.Url);
            //URL is returned blank if it is not a valid RSS/Atom Feed
            if (feedPost.Url == "")
            {
                return;
                //TODO Need to handle this to display feedback to the user
            }
            var feed = feedRepository.GetFeedByUrl(feedPost.Url);
            if (feed == null)
            {              
                feed = new Feed { Url = feedPost.Url};
                feedRepository.AddFeed(feed);
                
                var subscription = new Subscription
                {
                    UserId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId()),
                    FeedId = feed.Id
                };

                subscriptionRepository.AddSubscription(subscription);

                var fetcher = new Fetcher(_conStr);
                fetcher.FetchFeed(feed.Id);
            }
            else
            {
                var userId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
                var subscription = subscriptionRepository.GetSubscriptionByFeedAndUser(feed.Id,userId);
                if (subscription != null) return;
                subscription = new Subscription
                {
                    UserId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId()),
                    FeedId = feed.Id
                };
                subscriptionRepository.AddSubscription(subscription);
            }
            

        }
    }
}