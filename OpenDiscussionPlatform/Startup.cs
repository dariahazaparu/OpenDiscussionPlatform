using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OpenDiscussionPlatform.Startup))]
namespace OpenDiscussionPlatform
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
