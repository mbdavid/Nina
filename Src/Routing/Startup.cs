using Nina;
using System.Configuration;
using System.Web;
using System.Web.Routing;

namespace Nina
{
    public class Startup : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            var routes = RouteTable.Routes;

            var path = ConfigurationManager.AppSettings["Nina.Path"];
            var cors = ConfigurationManager.AppSettings["Nina.Cors"];

            if(string.IsNullOrEmpty(path)) path = "~/api";

            routes.Add(new MountingPoint(path, cors));
        }

        public void Dispose()
        {
        }
    }
}