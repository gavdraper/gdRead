using System.Web.Mvc;

namespace gdRead.Web.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult Woops()
        {
            return View();
        }
	}
}