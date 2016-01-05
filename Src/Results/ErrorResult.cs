using System;
using System.Net;
using System.Web;

namespace Nina
{
    public class ErrorResult : JsonResult
    {
        public ErrorResult(HttpStatusCode statusCode) :
            this(statusCode, null)
        {
        }

        public ErrorResult(Exception ex) :
            this((HttpStatusCode)(ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500), ex)
        {
        }

        public ErrorResult(HttpStatusCode statusCode, Exception exception)
            : base(new {
                code = (int)statusCode, 
                message = exception == null ? null : exception.Message, 
                type = exception == null ? statusCode.ToString() : exception.GetType().Name,
                stacktrace = exception == null ? null : exception.StackTrace
            })
        {
            this.StatusCode = (int)statusCode;
        }
    }
}