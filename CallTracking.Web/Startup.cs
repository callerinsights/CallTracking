using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CallTracking.Web.Startup))]
namespace CallTracking.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
