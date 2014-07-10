using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DapperExtensions;
using gdRead.Data.Models;
using gdRead.Data.Models.Dto;
using gdRead.Data.Repositories.Interfaces;

namespace gdRead.Data.Repositories
{
	public class PostRepository : IPostRepository
	{
		private readonly string _conStr;

		public PostRepository()
		{
			_conStr = ConfigurationManager.ConnectionStrings["gdRead.Data.gdReadContext"].ConnectionString;
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
		
		public void SetPostsInFeedAsRead(int feedId, Guid userId)
		{
			using (var con = new SqlConnection(_conStr))
			{
				con.Open();
				con.Query<DateTime?>(
					@"
					DECLARE @SubscriptionId INT
					SELECT @SubscriptionId = Id FROM Subscription WHERE UserId = @UserId AND FeedId = @FeedId

					INSERT INTO SubscriptionPostRead
					SELECT
						@SubscriptionId, Post.Id
					FROM
						Feed
						INNER JOIN SubScription ON Subscription.FeeDId = Feed.Id
						INNER JOIN Post ON Post.FeedId = Feed.Id 
						LEFT JOIN SubscriptionPostRead sr ON sr.postId = post.Id AND sr.SubscriptionId = Subscription.Id
					WHERE
						Subscription.UserId = @UserId AND
	
						Feed.Id = @FeedId AND
						sr.ID IS NULL
					"
					,
					new { FeedId = feedId, UserId = userId });
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
                        , CASE WHEN StarredPost.PostId IS NOT NULL THEN 1 ELSE 0 END AS [Starred]
					FROM 
						Post
						INNER JOIN Subscription ON Subscription.FeedId = Post.FeedId 
						LEFT JOIN SubscriptionPostRead ON SubscriptionPostRead.SubscriptionId = Subscription.Id AND SubscriptionPostRead.PostId = Post.Id
                        LEFT JOIN StarredPost ON StarredPost.UserId = Subscription.UserId AND StarredPost.PostId = Post.Id
					WHERE 
						post.FeedId = @FeedId
						AND Subscription.UserId = @UserId
					ORDER BY PublishDate DESC
					", new {FeedId = feedId, UserId = userId});
				con.Close();
				return posts;
			}
		}

		public string GetPostContent(int postId)
		{
			using (var con = new SqlConnection(_conStr))
			{
				con.Open();
				var post = con.Query<string>(@"
					SELECT 
						Post.Content
					FROM 
						Post                       
					WHERE 
						post.Id = @PostId
					", new { PostId = postId }).FirstOrDefault();
				con.Close();
				return post;
			} 
		}

		public IEnumerable<PostDto> GetPostDtoWithNameFromFeedWithoutContent(int feedId, Guid userId, int page,  bool unRead = false)
		{
			int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
			using (var con = new SqlConnection(_conStr))
			{
				con.Open();
				var posts = con.Query<PostDto>(@"
					SELECT 
						Post.Id, Post.Name, Post.Url, Post.PublishDate, Post.DateFetched, Post.FeedId
						, CASE WHEN SubscriptionPostRead.Id IS NOT NULL THEN 1 ELSE 0 END AS [Read]
                        , CASE WHEN StarredPost.PostId IS NOT NULL THEN 1 ELSE 0 END AS [Starred],
						Feed.Title As FeedTitle
					FROM 
						Post
						INNER JOIN Subscription ON Subscription.FeedId = Post.FeedId 
						LEFT JOIN SubscriptionPostRead ON SubscriptionPostRead.SubscriptionId = Subscription.Id AND SubscriptionPostRead.PostId = Post.Id
                        LEFT JOIN StarredPost ON StarredPost.UserId = Subscription.UserId AND StarredPost.PostId = Post.Id
						INNER JOIN Feed ON Feed.Id = Post.FeedId
					WHERE 
						post.FeedId = @FeedId
						AND Subscription.UserId = @UserId
						AND (@Unread = 0 OR SubscriptionPostRead.Id IS NULL)
					ORDER BY PublishDate DESC
					OFFSET @Page*@PageSize ROWS
					FETCH NEXT @PageSize ROWS ONLY"
                    , new { FeedId = feedId, UserId = userId, PageSize = pageSize, Page = page-1, Unread = unRead });
				con.Close();
				return posts;
			}
		}

		public IEnumerable<PostDto> GetPostDtoFromSubscriptionWithoutContent(Guid userId, int page, bool unRead = false)
		{
			int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
			using (var con = new SqlConnection(_conStr))
			{
				con.Open();
				var posts = con.Query<PostDto>(@"
					SELECT 
						Post.Id, Post.Name, Post.Url, Post.PublishDate, Post.DateFetched, Post.FeedId
						, CASE WHEN SubscriptionPostRead.Id IS NOT NULL THEN 1 ELSE 0 END AS [Read]
                        , CASE WHEN StarredPost.PostId IS NOT NULL THEN 1 ELSE 0 END AS [Starred]
						,Feed.Title As FeedTitle
					FROM 
						Post
						INNER JOIN Subscription ON Subscription.FeedId = Post.FeedId 
						LEFT JOIN SubscriptionPostRead ON SubscriptionPostRead.SubscriptionId = Subscription.Id AND SubscriptionPostRead.PostId = Post.Id
                        LEFT JOIN StarredPost ON StarredPost.UserId = Subscription.UserId AND StarredPost.PostId = Post.Id
						INNER JOIN Feed ON Feed.Id = Post.FeedId
					WHERE 
						Subscription.UserId = @UserId
						AND (@Unread = 0 OR SubscriptionPostRead.Id IS NULL)
					ORDER BY PublishDate DESC
					OFFSET @Page*@PageSize ROWS
					FETCH NEXT @PageSize ROWS ONLY"
                    , new {UserId = userId, PageSize = pageSize, Page = page-1, Unread = unRead });
				con.Close();
				return posts;
			}
		}

	    public int GetStarPortCount(Guid userId)
	    {
            using (var con = new SqlConnection(_conStr))
            {
                con.Open();
                var starCount = con.Query<int>(@"SELECT COUNT(*) FROM StarredPost WHERE UserId = @UserId", new { UserId = userId}).First();
                con.Close();
                return starCount;
            }
	    }

	    public void StarPost(int postId, Guid userId)
		{
			using (var con = new SqlConnection(_conStr))
			{
				con.Open();
				con.Execute(
					@"
						IF NOT EXISTS(SELECT UserId FROM StarredPost WHERE UserId = @UserId AND PostId = @PostId)
							BEGIN
							INSERT INTO StarredPost(UserId,PostId)
							VALUES(@UserId,@PostId)
							END
					",
					new { PostId = postId, UserId = userId });
				con.Close();
			}
		}

		public void UnStarPost(int postId, Guid userId)
		{
			using (var con = new SqlConnection(_conStr))
			{
				con.Open();
                con.Execute(@"DELETE FROM StarredPost WHERE UserId = @UserId AND PostID = @PostId",
					new { PostId = postId, UserId = userId });
				con.Close();
			}
		}


        public IEnumerable<Post> GetStarredPostsWithoutContent(Guid userId, int page)
        {
            int pageSize = int.Parse(ConfigurationManager.AppSettings["PageSize"]);
            using (var con = new SqlConnection(_conStr))
            {
                con.Open();
                var posts = con.Query<PostDto>(@"
					SELECT 
						Post.Id, Post.Name, Post.Url, Post.PublishDate, Post.DateFetched, Post.FeedId
						,0 AS [Read]
						,Feed.Title As FeedTitle
                        ,1 AS [Starred]
					FROM 
						Post
						INNER JOIN StarredPost ON StarredPost.PostId = Post.Id
						INNER JOIN Feed ON Feed.Id = Post.FeedId
					WHERE 
						StarredPost.UserId = @UserId						
					ORDER BY PublishDate DESC
					OFFSET @Page*@PageSize ROWS
					FETCH NEXT @PageSize ROWS ONLY"
                    , new { UserId = userId, PageSize = pageSize, Page = page - 1 });
                con.Close();
                return posts;
            }
        }
    }
}
