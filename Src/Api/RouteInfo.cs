using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;
using System.Security.Principal;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;

namespace Nina
{
    internal class RouteInfo
    {
        [JsonIgnore]
        public Regex Pattern { get; set; }
        public string Path { get; set; }
        public string Verb { get; set; }
        public string[] Roles { get; set; }
        public bool Authorize { get; set; }
        public bool Validate { get; set; }

        public string RegExPattern { get; set; }

        [JsonIgnore]
        public MethodInfo MethodInfo { get; set; }
        public string MethodName { get { return MethodInfo.Name + "(" + string.Join(", ", MethodInfo.GetParameters().Select(x => x.ParameterType.Name + " " + x.Name)) + ")"; } }
        public Type Module { get; set; }

        public RouteInfo(string mounted, string baseurl, string path, ParameterInfo[] pars)
        {
            var text = Regex.Replace(baseurl + path, @"^(.+)/$", "$1");

            this.Path = mounted + text;

            // creating regex por parameters
            foreach (var par in pars)
            {
                var name = par.Name;
                var expr = @"(?<$1>[^/]+)";

                // if parameter is a int, lets add regex (avoid 
                if (par.ParameterType == typeof(Int16) || par.ParameterType == typeof(Int32) || par.ParameterType == typeof(Int64)) // Inteiro
                {
                    expr = @"(?<$1>\d+)";
                }
                text = Regex.Replace(text, @"\{(" + name + @")\}", expr);

                // used for regex expression on route definition.
                // "/pubdate/{year:(\\d{4})}/{month:(\\d{2})"
                // "/code/{id:(\\[a-z]{8})}"
                // Syntax: "/demo/{parameterName:(regularExpression)}"

                text = Regex.Replace(text, @"\{(" + name + @"):\((.+?)\)\}", @"(?<$1>$2)");
            }

            this.RegExPattern = "^" + text + "$";
            this.Pattern = new Regex(this.RegExPattern);
        }

        public void ValidateAuthorizeAndRoles(IPrincipal user)
        {
            // lets check if user is authenticated
            if (this.Authorize && user.Identity.IsAuthenticated == false)
            {
                throw new HttpException(401, "Unauthorized");
            }

            // lets check if user has role for this method
            foreach (var role in this.Roles)
            {
                if (user.IsInRole(role))
                {
                    break;
                }
                throw new HttpException(403, "Forbidden");
            }
        }

        public object[] GetMethodParameters(Match match, HttpContext context)
        {
            var args = new List<object>();
            string model = null;

            // Parameters bind rules:
            // - if found on route pattern? Use route value
            // - if not found:
            //    - Parameter is `String`? Use `Request.Form.ToString()`
            //    - Parameter is `NameValueCollection`? Use `Request.Params` = Form + QueryString + Cookies + ServerVariables
            //    - Parameter is `Stream`? Use `Request.InputStream`
            //    - Parameter is `IPrincipal`? Use `HttpContext.User`
            //    - Parameter is `HttpContext`? Use `HttpContext`
            //    - Parameter is `HttpContextWrapper`? Use `new HttpContextWrapper(context)`
            //    - Parameter is `JObject`? Use `JObject.Parse()`
            //    - Parameter any other type? Use `JsonDeserialize(model, parameterType)`
            //    - Model is `Null`? Use `default(T)`

            // [Post("/order/{id}"]
            // void Edit(int id, OrderModel order, NameValueCollection form) { ... }

            foreach (ParameterInfo p in MethodInfo.GetParameters())
            {
                var pmatch = match.Groups[p.Name];

                if (pmatch.Success)
                {
                    var value = pmatch.Value;
                    args.Add(Convert.ChangeType(value, p.ParameterType));
                }
                else
                {
                    var request = context.Request;

                    if (p.ParameterType == typeof(NameValueCollection)) // bind NameValueCollection to Request.Params
                    {
                        args.Add(request.Params);
                    }
                    else if (p.ParameterType == typeof(Stream)) // bind Stream to Request.InputStream
                    {
                        args.Add(request.InputStream);
                    }
                    else if (p.ParameterType == typeof(HttpFileCollection)) // bind as HttpFileCollection
                    {
                        args.Add(request.Files);
                    }
                    else if (p.ParameterType == typeof(IPrincipal)) // bind as Context.User
                    {
                        args.Add(context.User);
                    }
                    else if (p.ParameterType == typeof(HttpContext)) // bind as context
                    {
                        args.Add(context);
                    }
                    else if (p.ParameterType == typeof(HttpContextWrapper)) // bind as HttpContextWrapper(context)
                    {
                        args.Add(new HttpContextWrapper(context));
                    }
                    else
                    {
                        if (model == null) // load request as string
                        {
                            model = this.GetModel(request.InputStream);
                        }

                        if (p.ParameterType == typeof(string)) // bind model as string 
                        {
                            args.Add(model);
                        }
                        else if(model == "") // no model in a custom parameter type
                        {
                            args.Add(GetDefault(p.ParameterType));
                        }
                        if (p.ParameterType == typeof(JObject)) // bind model as string 
                        {
                            args.Add(JObject.Parse(model));
                        }
                        else
                        {
                            args.Add(Newtonsoft.Json.JsonConvert.DeserializeObject(model, p.ParameterType));
                        }
                    }
                }
            }

            return args.Count == 0 ? null : args.ToArray();
        }

        private string GetModel(Stream stream)
        {
            if (this.Verb == "GET") return null;

            var model = "";

            using (var reader = new StreamReader(stream))
            {
                model = HttpUtility.UrlDecode(reader.ReadToEnd());
            }

            if (string.IsNullOrWhiteSpace(model)) model = "";

            // lets validate request
            if (this.Validate && model != "" && Regex.IsMatch(model, @"^(?!(.|\n)*<[a-z!\/?])(?!(.|\n)*&#)(.|\n)*$", RegexOptions.IgnoreCase) == false)
            {
                throw new HttpRequestValidationException("A potentially dangerous request value was detected from the client");
            }

            return model;
        }

        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
