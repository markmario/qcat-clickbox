
using Microsoft.Owin;

[assembly: OwinStartup(typeof(ClickBox.Web.Startup))]

namespace ClickBox.Web
{
    using Owin;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }

}