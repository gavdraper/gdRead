using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using gdRead.Data.Models;
using gdRead.Data.Repositories.Interfaces;
using Microsoft.AspNet.Identity;

namespace gdRead.Web.Controllers
{
    public class PostController : ApiController
    {
        private IPostRepository postRepository;

        public PostController(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        // GET api/<controller>
        public IEnumerable<Post> Get(int id, int page, string filter)
        {
            var filterByUnread = filter == "unread";
            var userId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
            return postRepository.GetPostDtoWithNameFromFeedWithoutContent(id, userId, page, filterByUnread);
        }

        public IEnumerable<Post> Get(int page, string filter)
        {
            var filterByUnread = filter == "unread";
            var userId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
            return postRepository.GetPostDtoFromSubscriptionWithoutContent(userId, page, filterByUnread);
        }

        public class PostContent
        {
            public int PostId { get; set; }
            public string Content { get; set; }
        }

        public PostContent Get(int postId, bool contentOnly)
        {
            return new PostContent()
            {
                PostId = postId,
                Content = postRepository.GetPostContent(postId)
            };
        }

    }
}