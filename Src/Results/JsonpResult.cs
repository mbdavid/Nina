using Newtonsoft.Json;
using System.Web;

namespace Nina
{
    public class JsonpResult : BaseResult
    {
        private string _callback;
        private object _result;

        public JsonpResult(object result)
            : this(result, "callback")
        {
        }

        public JsonpResult(object result, string callback)
        {
            this.ContentType = "application/x-javascript";
            _callback = callback;
            _result = result;
        }

        public override void Execute(HttpContext context)
        {
            base.Execute(context);

            context.Response.Write(string.Format("{0}(", _callback));

            if (_result == null)
            {
                context.Response.Write("null");
            }
            else
            {
                context.Response.Write(JsonConvert.SerializeObject(_result));
            } 

            context.Response.Write(");");
        }
    }
}