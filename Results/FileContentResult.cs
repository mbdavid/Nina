using System;
using System.IO;
using System.Web;

namespace Nina
{
    public class FileContentResult : Result
    {
        private Stream _stream;
        private byte[] _content = null;
        private long _length;
        private string _contentType;
        private string _filename;
        private bool _download;

        public FileContentResult(Stream stream, long length, string contentType, string filename = null, bool download = false)
        {
            _stream = stream;
            _length = length;
            _contentType = contentType;
            _filename = filename;
            _download = download;
        }

        public FileContentResult(byte[] content, string contentType, string filename = null, bool download = false)
        {
            if (content == null || content.Length == 0) throw new ArgumentNullException("content");
            _content = content;
            _length = _content.LongLength;
            _contentType = contentType;
            _filename = filename;
            _download = download;
        }

        public override void Execute(HttpContext context)
        {
            context.Response.Buffer = false;
            context.Response.BufferOutput = false;
            context.Response.ContentType = _contentType;
            context.Response.AppendHeader("Content-Length", _length.ToString());

            if (_download)
            {
                context.Response.AppendHeader("content-disposition", "attachment; filename=" + _filename);
            }

            if (_content != null)
            {
                context.Response.BinaryWrite(_content);
            }
            else
            {
                _stream.CopyTo(context.Response.OutputStream);
                _stream.Dispose();
            }
        }
    }
}