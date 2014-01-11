using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DapperExtensions;
using gdRead.Data.Models;
using Dapper;
using gdRead.Data.ViewModels;

namespace gdRead.Data.Repositories
{
    public class FeedRepository
    {
        private readonly string _conStr;
        public FeedRepository(string conStr)
        {
            _conStr = conStr;
        }

        public Feed AddFeed(Feed feed)
        {
            using (var con = new SqlConnection(_conStr))
            {
                con.Open();
                feed.Id = con.Insert(feed);                
                con.Close();
                return feed;
            }
        }

        public Feed GetFeedByUrl(string url)
        {
            using (var con = new SqlConnection(_conStr))
            {
                con.Open();
                var feed =  con.Query<Feed>("SELECT * FROM Feed WHERE URL = @Url",new {Url = url}).FirstOrDefault();
                con.Close();
                return feed;
            }            
        }

        public Feed UpdateFeed(Feed feed)
        {
            using (var con = new SqlConnection(_conStr))
            {
                con.Open();                                    
                con.Update(feed);
                con.Close();
                return feed;
            }
        }

        public Feed GetFeedById(int id)
        {
            using (var con = new SqlConnection(_conStr))
            {
                con.Open();
                var feed = con.Get<Feed>(id);
                con.Close();
                return feed;
            }            
        }

        public void UpdateLastchecked(int id)
        {
            using (var con = new SqlConnection(_conStr))
            {
                con.Open();
                con.Execute(@"UPDATE Feed SET LastChecked = GETDATE() WHERE Id = @FeedId", new { FeedId = id });
                con.Close();
            }   
        }

        public IEnumerable<Feed> GetSubscribedFeedsWithUnreadCount(Guid userId)
        {
            using (var con = new SqlConnection(_conStr))
            {
                con.Open();
                var feed = con.Query<FeedViewModel>(@"                    
                SELECT 
	                Feed.Id,Feed.Title,Feed.Url ,
	                SUM(CASE WHEN sr.Id IS NULL THEN 1 ELSE 0 END) As UnreadCount
                FROM 
	                Feed 
	                INNER JOIN Subscription ON Subscription.FeedId = Feed.Id
	                INNER JOIN Post ON Post.FeedId = Subscription.FeedId
	                LEFT JOIN SubscriptionPostRead sr ON sr.SubscriptionId = Subscription.Id AND sr.PostId = post.Id
                WHERE Subscription.UserId = @UserId
                GROUP BY 
	                Feed.Id,
	                Feed.Title,
	                Feed.Url
                ORDER BY Title
                ", new { UserId = userId });
                con.Close();
                return feed;
            }         
        }

        public IEnumerable<Feed> GetAllFeeds()
        {
            using (var con = new SqlConnection(_conStr))
            {
                con.Open();
                var feed = con.Query<Feed>(@"SELECT * FROM Feed");
                con.Close();
                return feed;
            }
        }

        public Subscription SubscribeToFeed(int feedId, Guid userId)
        {
            using (var con = new SqlConnection(_conStr))
            {
                con.Open();
                var subscription = new Subscription()
                {
                    UserId = userId,
                    FeedId = feedId
                };
                subscription.Id = con.Insert(subscription);
                con.Close();
                return subscription;
            }
        }
    }
}
