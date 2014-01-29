using System;
using System.Collections.Generic;
using gdRead.Data.Models;

namespace gdRead.Data.Repositories.Interfaces
{
    public interface IFeedRepository
    {
        Feed AddFeed(Feed feed);
        Feed GetFeedByUrl(string url);
        Feed UpdateFeed(Feed feed);
        Feed GetFeedById(int id);
        void UpdateLastchecked(int id);
        IEnumerable<Feed> GetSubscribedFeedsWithUnreadCount(Guid userId);
        IEnumerable<Feed> GetAllFeeds();
        Subscription SubscribeToFeed(int feedId, Guid userId);
    }
}
