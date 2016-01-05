using Nina;
using System;
using System.Collections.Specialized;
using System.Security.Principal;

namespace WebDemo
{
    [BaseUrl("/params")]
    public class UserApi : NinaModule
    {
        [Get("/user")]
        public string ShowMyUser(IPrincipal user)
        {
            return "Is user authenticated? " + user.Identity.IsAuthenticated;
        }

        [Get("/qs")]
        public object ShowAllForm(NameValueCollection qs)
        {
            return qs;
        }
    }
}