using System.Configuration;
using System.Web.Mvc;
using gdRead.Data.Repositories;
using gdRead.Data.Repositories.Interfaces;
using gdRead.FeedUtils;

namespace gdRead.Web.Controllers
{
    public class DevTestController : Controller
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IFeedRepository _feedRepository;
        private readonly IPostRepository _postRepository;

        public DevTestController(IDashboardRepository dashboardRepository, IFeedRepository feedRepository, IPostRepository postRepository)
        {
            this._dashboardRepository = dashboardRepository;
            this._feedRepository = feedRepository;
            this._postRepository = postRepository;
        }

        [Authorize(Roles = "Dev")]
        public ActionResult Dashboard()
        {
            return View(_dashboardRepository.GetDashboardStats());
        }

        public ActionResult ReFetchAll()
        {
            HttpContext.Server.ScriptTimeout = 300;
            var fetcher = new Fetcher(_feedRepository, _postRepository);
            fetcher.FetchAllFeeds();
            return RedirectToAction("MyFeeds", "Home");
        }
    }
}