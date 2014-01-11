using System.Configuration;
using System.Web.Mvc;
using gdRead.FeedUtils;

namespace gdRead.Web.Controllers
{
    public class DevTestController : Controller
    {
        private readonly string _conStr = ConfigurationManager.ConnectionStrings["gdRead.Data.gdReadContext"].ConnectionString;
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