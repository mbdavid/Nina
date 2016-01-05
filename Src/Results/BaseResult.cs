using System.Collections.Specialized;
using System.Web;

namespace Nina
{
    public class BaseResult
    {
        public static string Cors;

        public string ContentType { get; set; }
        public int? StatusCode { get; set; }
        public NameValueCollection Header { get; set; }

        public BaseResult()
        {
            this.Header = new NameValueCollection();
            this.ContentType = "application/json";
        }

        public virtual void Execute(HttpContext context)
        {
            var response = context.Response;

            response.ClearContent();
            response.ClearHeaders();

            response.TrySkipIisCustomErrors = true;

            if (this.StatusCode.HasValue)
            {
                response.StatusCode = StatusCode.Value;
            }

            foreach (string key in this.Header.Keys)
            {
                response.AppendHeader(key, Header[key]);
            }

            if (BaseResult.Cors != null)
            {
                response.AppendHeader("Access-Control-Allow-Origin", Cors);
            }

            response.ContentType = ContentType;
        }
    }
}