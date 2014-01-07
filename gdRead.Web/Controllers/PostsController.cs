using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Http;
using gdRead.Data;
using gdRead.Data.Models;
using gdRead.Data.Repositories;
using Microsoft.AspNet.Identity;

namespace gdRead.Web.Controllers
{     
    public class PostsController : ApiController
    {
        private readonly string _conStr = ConfigurationManager.ConnectionStrings["gdRead.Data.gdReadContext"].ConnectionString;
        // GET api/<controller>
       public IEnumerable<Post> Get(int id)
       {
           var postRepository = new PostRepository(_conStr);
           var userId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
           return postRepository.GetPostsFromFeed(id,userId);
       }

    }
}