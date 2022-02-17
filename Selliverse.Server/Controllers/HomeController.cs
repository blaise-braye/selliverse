using Microsoft.AspNetCore.Mvc;

namespace Selliverse.Server.Controllers
{
    public class HomeController : Controller
    {
        public string Index()
        {
            return "Hello world";
        }
    }
}
