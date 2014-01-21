using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using gdRead.Data.Models;
using gdRead.Data.Repositories;
using Microsoft.AspNet.Identity;

namespace gdRead.Web.Controllers
{     
    public class PostController : ApiController
    {
        private readonly string _conStr = ConfigurationManager.ConnectionStrings["gdRead.Data.gdReadContext"].ConnectionString;
        // GET api/<controller>
       public IEnumerable<Post> Get(int id)
       {
           var postRepository = new PostRepository(_conStr);
           var userId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
           return postRepository.GetPostsFromFeedWithoutContent(id, userId);
       }


        public class PostContent
        {
            public int PostId { get; set; }
            public string Content { get; set; }
        }

       public PostContent Get(int postId, bool contentOnly)
       {
           var postRepository = new PostRepository(_conStr);
           return new PostContent()
           {
               PostId = postId,
               Content = postRepository.GetPostContent(postId)
           };
       }

    }
}