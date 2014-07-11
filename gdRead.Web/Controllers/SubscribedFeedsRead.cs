using System;
using System.Web;
using System.Web.Http;
using gdRead.Data.Repositories.Interfaces;
using Microsoft.AspNet.Identity;

namespace gdRead.Web.Controllers
{
    public class SubscribedFeedsReadController : ApiController
    {
        private readonly IPostRepository _postRepository;

        public SubscribedFeedsReadController(IPostRepository postRepository)
        {
            this._postRepository = postRepository;
        }

        [Authorize]
        public void Post()
        {
            var userId = Guid.Parse(HttpContext.Current.User.Identity.GetUserId());
            _postRepository.SetAllAsRead(userId);
        }

    }
}
