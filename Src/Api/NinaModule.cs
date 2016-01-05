using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Principal;

namespace Nina
{
    public class NinaModule : IDisposable
    {
        public NinaModule()
        {
        }

        public virtual void OnException(Exception ex)
        {
        }

        public virtual void OnExecuting(MethodInfo methodInfo, object[] args)
        {
        }

        public virtual void OnExecuted(MethodInfo methodInfo, BaseResult result)
        {
        }

        #region Results Helpers

        protected BaseResult Ok()
        {
            return this.Status(HttpStatusCode.OK);
        }

        protected BaseResult Status(HttpStatusCode statusCode)
        {
            var result = new BaseResult();
            result.StatusCode = (int)statusCode;
            return result;
        }

        protected BaseResult Status(int statusCode)
        {
            var result = new BaseResult();
            result.StatusCode = statusCode;
            return result;
        }

        protected BaseResult Redirect(string url)
        {
            var result = new BaseResult();
            result.StatusCode = 302;
            result.Header["Location"] = url;
            return result;
        }

        protected ErrorResult Error(Exception exception)
        {
            return new ErrorResult(exception);
        }

        protected ErrorResult Error(string message)
        {
            return new ErrorResult(new Exception(message));
        }

        protected ErrorResult Error(HttpStatusCode statusCode)
        {
            return new ErrorResult(statusCode, null);
        }

        protected ErrorResult Error(int statusCode)
        {
            return new ErrorResult((HttpStatusCode)statusCode, null);
        }

        protected FileContentResult File(byte[] content, string contenttype)
        {
            return new FileContentResult(content, contenttype);
        }

        protected FileContentResult File(Stream stream, long length, string contenttype)
        {
            return new FileContentResult(stream, length, contenttype);
        }

        protected FileResult File(string path)
        {
            return new FileResult(path);
        }

        protected FileContentResult Download(Stream stream, long length, string filename)
        {
            return new FileContentResult(stream, length, "application/octet-stream", filename, true);
        }

        protected FileContentResult Download(byte[] content, string filename)
        {
            return new FileContentResult(content, "application/octet-stream", filename, true);
        }

        protected TextResult Text(string text)
        {
            return new TextResult(text);
        }

        protected TextResult Text(string text, string contentType)
        {
            return new TextResult(text, contentType);
        }

        protected TextResult Html(string text)
        {
            return new TextResult(text, "text/html");
        }

        protected JsonResult Nothing()
        {
            return new JsonResult(null);
        }

        protected JsonResult Json(object model)
        {
            return new JsonResult(model);
        }

        protected JsonpResult Jsonp(object model, string callback = "callback")
        {
            return new JsonpResult(model, callback);
        }

        #endregion

        public virtual void Dispose()
        {
        }
    }
}
