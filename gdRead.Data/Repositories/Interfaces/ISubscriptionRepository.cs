using System;
using gdRead.Data.Models;

namespace gdRead.Data.Repositories.Interfaces
{
    public interface ISubscriptionRepository
    {
        Subscription AddSubscription(Subscription subscription);
        void Unsubscribe(int feedId, Guid userId);
        Subscription GetSubscriptionByFeedAndUser(int feedId, Guid userId);
    }
}
