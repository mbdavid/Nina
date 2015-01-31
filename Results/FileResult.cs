using System;
using System.IO;
using System.Web;

namespace Nina
{
    public class FileResult : Result
    {
        private readonly string _path;

        public FileResult(string path)
        {
            _path = path;
        }

        public override void Execute(HttpContext context)
        {
            context.Response.TransmitFile(_path);
        }
    }
}