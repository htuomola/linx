using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(LinkLogger.Startup))]
namespace LinkLogger
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
    