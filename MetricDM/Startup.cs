using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MetricDM.App_Start.Startup))]
namespace MetricDM.App_Start
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
