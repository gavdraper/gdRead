using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using gdRead.Data.Models;
using gdRead.Data.Repositories.Interfaces;
using Microsoft.AspNet.Identity;

namespace gdRead.Web.Controllers
{
    public class StaredPostController : ApiController
    {
        private IPostRepository _postRepository;

        public StaredPostController(IPostRepository postRepository)
        {
            this._postRepository = postRepository;
        }

        public IEnumerable<Post> Get(int id)
        {
            var userId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
            return _postRepository.GetStaredPostsWithoutContent(userId,id);
        }

        public void Post(int id)
        {
            var userId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
            _postRepository.StarPost(id, userId);
        }

        public void Delete(int id)
        {
             var userId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
            _postRepository.UnStarPost(id,userId);
        }
    }
}