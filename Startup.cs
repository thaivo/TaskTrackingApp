using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TaskTrackingApp.Startup))]
namespace TaskTrackingApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
