using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MvcPractice.Startup))]
namespace MvcPractice
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
