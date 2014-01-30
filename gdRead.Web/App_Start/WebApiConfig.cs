using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using gdRead.Data.Repositories;
using gdRead.Data.Repositories.Interfaces;

namespace gdRead.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "StaredPosts",
                routeTemplate: "api/Post/Stared/{id}",
                defaults: new { controller = "StaredPostController"}
            );


    
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
                routeTemplate: "api/Feed/{id}/Post/Page/{page}/Filter/{filter}",
                defaults: new { controller = "Post" }
            );

            config.Routes.MapHttpRoute(
                name: "SubscriptionPosts",
                routeTemplate: "api/Post/Page/{page}/Filter/{filter}",
                defaults: new { controller = "Post" }
            );

  

            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.Register(x => new DashboardRepository()).As<IDashboardRepository>().InstancePerApiRequest();
            builder.Register(x => new SubscriptionRepository()).As<ISubscriptionRepository>().InstancePerApiRequest();
            builder.Register(x => new PostRepository()).As<IPostRepository>().InstancePerApiRequest();
            builder.Register(x => new FeedRepository()).As<IFeedRepository>().InstancePerApiRequest();
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
