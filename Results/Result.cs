using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;

namespace Nina
{
    public class Result
    {
        public static string Cors;

        public string ContentType { get; set; }
        public int? StatusCode { get; set; }
        public NameValueCollection Header { get; set; }

        public Result()
        {
            Header = new NameValueCollection();
            ContentType = "application/json";
        }

        public virtual void Execute(HttpContext context)
        {
            var response = context.Response;
            if(StatusCode.HasValue) response.StatusCode = StatusCode.Value;

            foreach (string key in Header.Keys)
            {
                response.AppendHeader(key, Header[key]);
            }

            response.ContentType = ContentType;

            if (Cors != null)
            {
                response.AppendHeader("Access-Control-Allow-Origin", Cors);
            }
        }
    }
}