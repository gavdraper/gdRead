using System.Web.Mvc;
using System.Web.Routing;

namespace gdRead.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "UnAuthenticatedHomePage",
                url: "Home/",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapRoute(
                name: "Dashboard",
                url: "Dev/",
                defaults: new { controller = "DevTest", action = "Dashboard" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "MyFeeds", id = UrlParameter.Optional }
            );
        }
    }
}
