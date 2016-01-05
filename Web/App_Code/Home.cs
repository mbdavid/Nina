using Nina;
using System.Web;

namespace WebDemo
{
    [BaseUrl("/home")]
    public class HomeApi : NinaModule
    {
        [Get("/")]
        public string Index()
        {
            return "My Home Api";
        }

        [Get("/{id}")]
        public string View(string id)
        {
            return "your id is " + id;
        }
    }
}