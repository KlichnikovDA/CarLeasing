using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CarLeasing.Startup))]
namespace CarLeasing
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
