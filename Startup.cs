using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GroupBuilderAdmin.Startup))]
namespace GroupBuilderAdmin
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
