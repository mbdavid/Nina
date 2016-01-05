using System;

namespace Nina
{
    public class ValidateInputAttribute : Attribute
    {
        public bool Validate { get; set; }

        public ValidateInputAttribute()
        {
            this.Validate = true;
        }

        public ValidateInputAttribute(bool validate)
        {
            this.Validate = validate;
        }
    }

    public class AuthorizeAttribute : Attribute
    {
        public bool NeedAuthorize { get; set; }

        public AuthorizeAttribute()
        {
            this.NeedAuthorize = true;
        }

        public AuthorizeAttribute(bool needAuthorize)
        {
            this.NeedAuthorize = needAuthorize;
        }
    }

    public class RoleAttribute : Attribute
    {
        public string[] Roles { get; set; }

        public RoleAttribute(params string[] roles)
        {
            this.Roles = roles;
        }
    }

    public class BaseUrlAttribute : Attribute
    {
        public string Path { get; set; }

        public BaseUrlAttribute(string path)
        {
            if(string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            if (!path.StartsWith("/") || path.EndsWith("/")) throw new ArgumentException("BaseUrl path must starts with / (ex: /customers)");
            if (path.Length > 1 && path.EndsWith("/")) throw new ArgumentException("BaseUrl path must not ends with a / (ex: /customers)");

            this.Path = path;
        }
    }

    public abstract class HttpVerbAttribute : Attribute
    {
        public abstract string Verb { get; }
        public string Path { get; set; }

        public HttpVerbAttribute(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            if (!path.StartsWith("/")) throw new ArgumentException("Verb path must starts with / (ex: /view/{id})");
            if (path.Length > 1 && path.EndsWith("/")) throw new ArgumentException("Verb path must not ends with / (ex: /view/{id})");

            this.Path = path;
        }
    }

    public class GetAttribute : HttpVerbAttribute
    {
        public override string Verb { get { return "GET"; } }
        public GetAttribute(string path) : base(path) { }
    }

    public class PostAttribute : HttpVerbAttribute
    {
        public override string Verb { get { return "POST"; } }
        public PostAttribute(string path) : base(path) { }
    }

    public class DeleteAttribute : HttpVerbAttribute
    {
        public override string Verb { get { return "DELETE"; } }
        public DeleteAttribute(string path) : base(path) { }
    }

    public class PutAttribute : HttpVerbAttribute
    {
        public override string Verb { get { return "PUT"; } }
        public PutAttribute(string path) : base(path) { }
    }
}
