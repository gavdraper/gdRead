using System;
using System.Web;
using System.Web.Http;
using gdRead.Data.Models;
using gdRead.Data.Repositories.Interfaces;
using Microsoft.AspNet.Identity;

namespace gdRead.Web.Controllers
{
    public class FeedReadController : ApiController
    {
        private readonly IPostRepository _postRepository;

        public FeedReadController(IPostRepository postRepository)
        {
            this._postRepository = postRepository;
        }


        [Authorize]
        public void Post([FromBody] Feed feed)
        {
            var userId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
            _postRepository.SetPostsInFeedAsRead(feed.Id, userId);
        }
    }
}
