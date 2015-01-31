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
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Serialization;

namespace Nina
{
    public class JsonResult : Result
    {
        private object _result;

        public JsonResult(object result)
        {
            ContentType = "application/json";
            _result = result;
        }

        public override void Execute(HttpContext context)
        {
            base.Execute(context);

            if (_result == null) return;

            context.Response.Write(JsonConvert.SerializeObject(_result));
        }
    }
}