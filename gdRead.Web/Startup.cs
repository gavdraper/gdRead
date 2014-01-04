using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(gdRead.Web.Startup))]
namespace gdRead.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
