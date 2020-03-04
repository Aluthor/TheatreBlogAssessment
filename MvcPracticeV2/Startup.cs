using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MvcPracticeV2.Startup))]
namespace MvcPracticeV2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
