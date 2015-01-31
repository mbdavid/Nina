using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Web;
using System.Linq;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq.Expressions;

namespace Nina
{
    public class JsonpResult : JsonResult
    {
        private string _callback;

        public JsonpResult(object result)
            : this(result, "callback")
        {
        }

        public JsonpResult(object result, string callback)
            : base(result)
        {
            this.ContentType = "application/x-javascript";
            _callback = callback;
        }

        public override void Execute(HttpContext context)
        {
            context.Response.Write(string.Format("{0}(", _callback));
            base.Execute(context);
            context.Response.Write(");");
        }
    }
}