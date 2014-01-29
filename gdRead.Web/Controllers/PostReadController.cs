using System;
using System.Web;
using System.Web.Http;
using gdRead.Data.Models;
using gdRead.Data.Repositories.Interfaces;
using Microsoft.AspNet.Identity;

namespace gdRead.Web.Controllers
{
    public class PostReadController : ApiController
    {
        private IPostRepository postRepository;

        public PostReadController(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        [Authorize]
        public void Post([FromBody] Post post)
        {
            var userId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
            postRepository.SetPostAsRead(post.Id, userId);
        }
    }
}