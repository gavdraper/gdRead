using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace gdRead.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();


            config.Routes.MapHttpRoute(
                name: "PostNoContent",
                routeTemplate: "api/{controller}/ContentOnly/{postId}/{contentOnly}",
                defaults: new { contentOnly = true }
            );


            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            config.Routes.MapHttpRoute(
                name: "FeedPosts",
                routeTemplate: "api/Feed/{id}/Post/Page/{page}",
                defaults: new {controller="Post" }
            );

            config.Routes.MapHttpRoute(
                name: "SubscriptionPosts",
                routeTemplate: "api/Post/Page/{page}",
                defaults: new { controller = "Post" }
            );


            /*
            routes.MapRoute(
                name: "PostFromFeed",
                url: "{controller}/{action}/FromFeed/{feedId}",
                defaults: new { controller = "Post", action = "Index" }
            );
*/
        }
    }
}
