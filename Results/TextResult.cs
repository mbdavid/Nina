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
            this.ContentType = "plain/text";
            _text = text;
        }

        public override void Execute(HttpContext context)
        {
            base.Execute(context);
            context.Response.Write(_text);
        }
    }
}