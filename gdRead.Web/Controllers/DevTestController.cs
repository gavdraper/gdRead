using System.Configuration;
using System.Web.Mvc;
using gdRead.Data.Repositories;
using gdRead.FeedUtils;

namespace gdRead.Web.Controllers
{
    public class DevTestController : Controller
    {
        private readonly string _conStr = ConfigurationManager.ConnectionStrings["gdRead.Data.gdReadContext"].ConnectionString;




        [Authorize(Roles = "Dev")]
        public ActionResult Dashboard()
        {
            var repo = new DashboardRepository(_conStr);


            return View(repo.GetDashboardStats());
        }


        //
        // GET: /DevTest/
        
        public ActionResult ReFetchAll()
        {
            HttpContext.Server.ScriptTimeout = 300;
            var fetcher = new Fetcher(_conStr);
            fetcher.FetchAllFeeds();
            return RedirectToAction("MyFeeds", "Home");
        }
    }
}