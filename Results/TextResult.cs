using System;
using System.IO;
using System.Text;
using System.Web;

namespace Nina
{
    public class TextResult : BaseResult
    {
        private readonly string _text;

        public TextResult(string text)
        {
            _text = text;
            ContentType = "plain/text";
        }

        public override void Execute(HttpContext context)
        {
            base.Execute(context);
            context.Response.Write(_text);
        }
    }
}