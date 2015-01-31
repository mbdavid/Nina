using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using Nina.Routing;
using System.Net;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Nina
{
    internal class ModuleHandler : IHttpHandler
    {
        internal static List<RouteInfo> Routes { get; set; }

        public string MountingPointPath { get; set; }

        public ModuleHandler(string mountedUrl)
        {
            MountingPointPath = mountedUrl;
        }

        public void ProcessRequest(HttpContext context)
        {
            var verb = context.Request.HttpMethod.ToUpper();
            var reqUri = context.Request.Url.AbsolutePath;
            var baseUri = CreateBaseUrl(context.Request.Url, MountingPointPath).AbsolutePath;
            var path = Regex.Replace(reqUri.Substring(baseUri.Length), @"^(.+)/$", "$1");

            if (string.IsNullOrEmpty(path)) path = "/"; // if empty path, its /
            if (!path.StartsWith("/")) path = "/" + path; // path must start with /

            // debug
            if (path == "/wsdl" && context.Request.IsLocal)
            {
                new JsonResult(Routes).Execute(context);
                return;
            }

            Module instance = null;

            try
            {
                Match match;
                var ri = GetRouteInfo(verb, path, out match);

                // validate authorize and roles
                ri.ValidateAuthorizeAndRoles(context.User);

                // get method parameter to execute
                var args = ri.GetMethodParameters(match, context.Request);

                // create object instance
                instance = (Module)Activator.CreateInstance(ri.Module);

                // bind user from context
                instance.User = context.User;

                // before execute
                instance.OnExecuting(ri.MethodInfo, args);

                // execute method
                var obj = ri.MethodInfo.Invoke(instance, args);

                // if result is BaseResult, use it, otherwise, convert to JsonResult
                var result = obj is Result ? (Result)obj : new JsonResult(obj);

                // Aater execute, passing Method + BaseResult
                instance.OnExecuted(ri.MethodInfo, result);

                // rendering result (when result == null is void)
                if (result != null)
                {
                    result.Execute(context);
                }
            }
            catch (TargetInvocationException ex)
            {
                if (instance != null)
                {
                    instance.OnException(ex.InnerException);
                }
                new ErrorResult(ex.InnerException).Execute(context);
            }
            catch (Exception ex)
            {
                if (instance != null)
                {
                    instance.OnException(ex);
                }
                new ErrorResult(ex).Execute(context);
            }
            finally
            {
                if (instance != null)
                {
                    instance.Dispose();
                }
            }
        }

        private static Uri CreateBaseUrl(Uri url, string mountingPointPath)
        {
            return new Uri(string.Format("http{0}://{1}{2}", 
                HttpContext.Current.Request.IsSecureConnection ? "s" : "",
                url.Authority, 
                VirtualPathUtility.ToAbsolute(mountingPointPath)));
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public static RouteInfo GetRouteInfo(string verb, string path, out Match match)
        {
            var ex = new HttpException(404, "Not found");

            foreach (var m in Routes)
            {
                match = m.Pattern.Match(path);

                if (!match.Success) continue; // not found, read next

                ex = new HttpException(405, "Method Not Allowed");

                if (m.Verb != verb) continue; // found but not with the same method, continue searching

                return m;
            }

            throw ex;
        }
    }
}
