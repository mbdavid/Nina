using Nina;
using System;
using System.Web;

namespace WebDemo
{
    public class IndexApi : NinaModule
    {
        [Get("/index")]
        public string Index()
        {
            return "My Nina Index";
        }

        [Get("/now")]
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}