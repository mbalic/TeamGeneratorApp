using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TeamGeneratorApp.Startup))]
namespace TeamGeneratorApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
