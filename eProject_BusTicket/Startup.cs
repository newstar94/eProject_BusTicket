using Owin;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(eProject_BusTicket.Startup))]
namespace eProject_BusTicket
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
