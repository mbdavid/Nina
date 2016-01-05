using System.Web;

namespace Nina
{
    public class TextResult : BaseResult
    {
        private readonly string _text;

        public TextResult(string text, string contentType = "plain/text")
        {
            this.ContentType = contentType;
            _text = text;
        }

        public override void Execute(HttpContext context)
        {
            base.Execute(context);

            context.Response.Write(_text);
        }
    }
}