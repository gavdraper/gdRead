using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using gdRead.Data.Models;
using gdRead.Data.Repositories;
using gdRead.Data.Repositories.Interfaces;
using gdRead.FeedUtils;
using gdRead.Web.Models.DTOs;
using Microsoft.AspNet.Identity;

namespace gdRead.Web.Controllers
{
    public class FeedController : ApiController
    {
        private readonly IFeedRepository _feedRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IPostRepository _postRepository;

        public FeedController(IFeedRepository feedRepository, ISubscriptionRepository subscriptionRepository, IPostRepository postRepository)
        {
            this._feedRepository = feedRepository;
            this._subscriptionRepository = subscriptionRepository;
            this._postRepository = postRepository;
        }

        [Authorize]
        public IEnumerable<Feed> Get()
        {

            var userId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());            
            return _feedRepository.GetSubscribedFeedsWithUnreadCount(userId);
        }

        [Authorize]
        public void Delete(int id)
        {
            var userId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
            _subscriptionRepository.Unsubscribe(id,userId);
        }
        
        [Authorize]
        public void Post([FromBody] FeedPostModel feedPost)
        {
            feedPost.Url = FeedUrlFinder.FindFeedUrl(feedPost.Url);
            //URL is returned blank if it is not a valid RSS/Atom Feed
            if (feedPost.Url == "")
            {
                return;
                //TODO Need to handle this to display feedback to the user
            }
            var feed = _feedRepository.GetFeedByUrl(feedPost.Url);
            if (feed == null)
            {              
                feed = new Feed { Url = feedPost.Url};
                _feedRepository.AddFeed(feed);
                
                var subscription = new Subscription
                {
                    UserId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId()),
                    FeedId = feed.Id
                };

                _subscriptionRepository.AddSubscription(subscription);

                var fetcher = new Fetcher(_feedRepository, _postRepository);
                fetcher.FetchFeed(feed.Id);
            }
            else
            {
                var userId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
                var subscription = _subscriptionRepository.GetSubscriptionByFeedAndUser(feed.Id,userId);
                if (subscription != null) return;
                subscription = new Subscription
                {
                    UserId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId()),
                    FeedId = feed.Id
                };
                _subscriptionRepository.AddSubscription(subscription);
            }
            

        }
    }
}