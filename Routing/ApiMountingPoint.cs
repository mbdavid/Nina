using System;
using System.Web;
using System.Web.Routing;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nina.Routing
{
	public class MountingPoint : RouteBase
	{
		private readonly string _mountedUrl;

        public MountingPoint(string mountedUrl, string cors = null)
	    {
            _mountedUrl = mountedUrl;

            if (!mountedUrl.StartsWith("~") || mountedUrl.EndsWith("/"))
            {
                throw new ApplicationException("ApiMountingPoint: mountedUrl MUST starts with ~ and NOT ends with /");
            }

            // getting all classes that implements "Controllers"
            Result.Cors = cors;

            var routes = ModuleHandler.Routes = new List<RouteInfo>();
            var baseType = typeof(Module);
            var types = AppDomain.CurrentDomain.GetAssemblies().ToList()
                .SelectMany(s => s.GetTypes())
                .Where(p => baseType.IsAssignableFrom(p) && !p.IsAbstract);

            foreach (var type in types)
            {
                var authorize = (AuthorizeAttribute)type.GetCustomAttributes(typeof(AuthorizeAttribute), true).FirstOrDefault();
                var baseurl = (BaseUrlAttribute)type.GetCustomAttributes(typeof(BaseUrlAttribute), true).FirstOrDefault();

                foreach (var method in type.GetMethods().Where(x => x.IsPublic))
                {
                    var attr = (HttpVerbAttribute)method.GetCustomAttributes(typeof(HttpVerbAttribute), true).FirstOrDefault();

                    if (attr != null)
                    {
                        var pars = method.GetParameters();

                        var route = new RouteInfo(_mountedUrl, baseurl == null ? "" : baseurl.Path, attr.Path, pars);
                        route.Verb = attr.Verb;
                        route.Module = type;
                        route.MethodInfo = method;
                        route.Authorize = authorize != null && authorize.NeedAuthorize;
                        route.Validate = true;

                        // check for authorize attribute - in class or in method
                        var mauthorize = (AuthorizeAttribute)method.GetCustomAttributes(typeof(AuthorizeAttribute), true).FirstOrDefault();

                        if (mauthorize != null)
                        {
                            route.Authorize = mauthorize.NeedAuthorize;
                        }

                        var mvalidate = (ValidateInputAttribute)method.GetCustomAttributes(typeof(ValidateInputAttribute), true).FirstOrDefault();

                        if (mvalidate != null)
                        {
                            route.Validate = mvalidate.Validate;
                        }

                        // get roles attributes
                        var roles = (RoleAttribute)method.GetCustomAttributes(typeof(RoleAttribute), true).FirstOrDefault();

                        route.Roles = roles != null ? roles.Roles : new string[0];

                        routes.Add(route);
                    }
                }
            }
        }

		public override RouteData GetRouteData(HttpContextBase httpContext)
		{
            if(!httpContext.Request.AppRelativeCurrentExecutionFilePath.StartsWith(_mountedUrl))
				return null;

            return new RouteData(this, new MountedRouteHandler(_mountedUrl));
		}

	    public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
		{
			return null;
		}
	}
}