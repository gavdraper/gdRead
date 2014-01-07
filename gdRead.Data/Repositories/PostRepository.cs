﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DapperExtensions;
using gdRead.Data.Models;

namespace gdRead.Data.Repositories
{
    public class PostRepository
    {
        private readonly string _conStr;

        public PostRepository(string conStr)
        {
            _conStr = conStr;
        }

        public DateTime GetLastPostDateInFeed(int feedId)
        {
            using (var con = new SqlConnection(_conStr))
            {
                con.Open();
                var lastPostDate = con.Query<DateTime?>("SELECT TOP 1 PublishDate FROM Post WHERE FeedId = @FeedId ORDER BY PublishDate DESC", new { FeedId =  feedId}).FirstOrDefault() ?? DateTime.MinValue;
                con.Close();
                return lastPostDate;
            }
        }

        public void SetPostAsRead(int postId, Guid userId)
        {
            using (var con = new SqlConnection(_conStr))
            {
                con.Open();
                con.Query<DateTime?>(
                    @"
                        DECLARE @FeedId INT 
                        SELECT @FeedId = FeedId FROM Post WHERE Id = @postId
                        DECLARE @SubscriptionId INT
                        SELECT @SubscriptionId = Id FROM Subscription WHERE UserId = @UserId AND FeedId = @FeedId
                        IF NOT EXISTS(SELECT Id FROM SubscriptionPostRead WHERE SubscriptionId = @SubscriptionId AND PostId = @PostId)
	                        BEGIN
		                    INSERT INTO SubscriptionPostRead(SubScriptionId,PostId)
		                    VALUES(@SubscriptionId,@PostId)
	                        END
                    "
                    ,
                    new {PostId = postId, UserId = userId});
                con.Close();
            }
        }

        public Post AddPost(Post post)
        {
            using (var con = new SqlConnection(_conStr))
            {
                con.Open();
                post.Id = con.Insert(post);
                con.Close();
                return post;
            }
        }

        public IEnumerable<Post> GetPostsFromFeed(int feedId, Guid userId)
        {
            using (var con = new SqlConnection(_conStr))
            {
                con.Open();
                var posts = con.Query<Post>(@"
                    SELECT 
	                    Post.*
	                    , CASE WHEN SubscriptionPostRead.Id IS NOT NULL THEN 1 ELSE 0 END AS [Read]
                    FROM 
	                    Post
                        INNER JOIN Subscription ON Subscription.FeedId = Post.FeedId 
	                    LEFT JOIN SubscriptionPostRead ON SubscriptionPostRead.SubscriptionId = Subscription.Id AND SubscriptionPostRead.PostId = Post.Id
                    WHERE 
	                    post.FeedId = @FeedId
	                    AND Subscription.UserId = @UserId
                    ", new {FeedId = feedId, UserId = userId});
                con.Close();
                return posts;
            }
        }
    }
}
