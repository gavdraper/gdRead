using System;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DapperExtensions;
using gdRead.Data.Models;

namespace gdRead.Data.Repositories
{
    public class SubscriptionRepository
    {
        private readonly string _conStr;
        public SubscriptionRepository(string conStr)
        {
            _conStr = conStr;
        }

        public Subscription AddSubscription(Subscription subscription)
        {
            using (var con = new SqlConnection(_conStr))
            {
                con.Open();
                subscription.Id = con.Insert(subscription);
                con.Close();
                return subscription;
            }
        }

        public void Unsubscribe(int feedId, Guid userId)
        {
            using (var con = new SqlConnection(_conStr))
            {
                con.Open();                
                con.Execute(@"
                    DECLARE @SubId INT
                    SELECT @SubId = Id FROM Subscription WHERE FeedId = @FeedId AND UserId = @UserId
                    DELETE FROM SubscriptionPostRead WHERE SubscriptionId = @SubId
                    DELETE FROM Subscription WHERE id = @SubId", new { UserId = userId, FeedId = feedId });
                con.Close();
            }            
        }


        public Subscription GetSubscriptionByFeedAndUser(int feedId, Guid userId)
        {
            using (var con = new SqlConnection(_conStr))
            {
                con.Open();
                var subscription = con.Query<Subscription>("SELECT * FROM Subscription WHERE feedId = @FeedId AND userId = @UserId", new { UserId = userId, FeedId=feedId }).FirstOrDefault();
                con.Close();
                return subscription;
            }
        }
    }
}
