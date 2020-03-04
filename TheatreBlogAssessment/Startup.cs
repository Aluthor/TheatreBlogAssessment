using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TheatreBlogAssessment.Startup))]
namespace TheatreBlogAssessment
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
