using System;
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

        public IEnumerable<Post> GetPostsFromFeed(int feedId)
        {
            using (var con = new SqlConnection(_conStr))
            {
                con.Open();
                var posts = con.Query<Post>("SELECT * FROM Post WHERE FeedId = @FeedId", new {FeedId = feedId});
                con.Close();
                return posts;
            }
        }
    }
}
