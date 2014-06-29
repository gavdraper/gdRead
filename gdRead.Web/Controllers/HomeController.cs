using System.Web.Mvc;

namespace gdRead.Web.Controllers
{

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("MyFeeds");
            return View();
        }

        [Authorize]
        public ActionResult MyFeeds()
        {
            return View();
        }


    }
}