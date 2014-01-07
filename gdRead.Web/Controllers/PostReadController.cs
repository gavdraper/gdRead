using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using gdRead.Data;
using gdRead.Data.Models;
using Microsoft.AspNet.Identity;

namespace gdRead.Web.Controllers
{
	public class PostReadController : ApiController
	{
		[Authorize]
		public void Post([FromBody] Post post)
		{
			var ctx = new gdReadContext();
			var userId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
			var dbPost = ctx.Posts.FirstOrDefault(x => x.Id == post.Id);
			var sub = ctx.Subscriptions.FirstOrDefault(x => x.Feed.Id == post.Feed.Id && x.UserId == userId);
			var subRead = new SubscriptionReadPost()
			{
				Post = dbPost,
				Subscription = sub
			};
			ctx.SubscriptionsSubscriptionReadPost.Add(subRead);
			ctx.SaveChanges();
		}
	}
}