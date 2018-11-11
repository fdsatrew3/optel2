using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Optel2.Startup))]
namespace Optel2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
