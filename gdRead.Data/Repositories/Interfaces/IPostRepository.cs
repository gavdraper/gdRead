using System;
using System.Collections.Generic;
using gdRead.Data.Models;
using gdRead.Data.Models.Dto;

namespace gdRead.Data.Repositories.Interfaces
{
    public interface IPostRepository
    {
        DateTime GetLastPostDateInFeed(int feedId);
        void SetPostAsRead(int postId, Guid userId);
        void StarPost(int postId, Guid userId);
        void UnStarPost(int postId, Guid userId);
        IEnumerable<Post> GetStarredPostsWithoutContent(Guid userId, int page);
        void SetPostsInFeedAsRead(int feedId, Guid userId);
        Post AddPost(Post post);
        IEnumerable<Post> GetPostsFromFeed(int feedId, Guid userId);
        string GetPostContent(int postId);
        IEnumerable<PostDto> GetPostDtoWithNameFromFeedWithoutContent(int feedId, Guid userId, int page, bool unRead = false);
        IEnumerable<PostDto> GetPostDtoFromSubscriptionWithoutContent(Guid userId, int page, bool unRead = false);
    }
}
