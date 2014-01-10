﻿using System;
using System.Configuration;
using System.Web;
using System.Web.Http;
using gdRead.Data.Models;
using gdRead.Data.Repositories;
using Microsoft.AspNet.Identity;

namespace gdRead.Web.Controllers
{
    public class PostReadController : ApiController
    {
        private readonly string _conStr =
            ConfigurationManager.ConnectionStrings["gdRead.Data.gdReadContext"].ConnectionString;

        [Authorize]
        public void Post([FromBody] Post post)
        {
            var postRepository = new PostRepository(_conStr);
            var userId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
            postRepository.SetPostAsRead(post.Id, userId);
        }
    }
}