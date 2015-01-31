using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Net;
using System.Reflection;

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
            this.Path = path;
        }
    }

    public abstract class HttpVerbAttribute : Attribute
    {
        public abstract string Verb { get; }
        public string Path { get; set; }

        public HttpVerbAttribute(string path)
        {
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
