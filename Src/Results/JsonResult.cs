using Newtonsoft.Json;
using System.Web;

namespace Nina
{
    public class JsonResult : BaseResult
    {
        private object _result;

        public JsonResult(object result)
        {
            this.ContentType = "application/json";
            _result = result;
        }

        public override void Execute(HttpContext context)
        {
            base.Execute(context);

            if (_result == null)
            {
                context.Response.Write("null");
            }
            else
            {
                context.Response.Write(JsonConvert.SerializeObject(_result));
            }
        }
    }
}