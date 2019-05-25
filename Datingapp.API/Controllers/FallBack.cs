using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Datingapp.API.Controllers
{
    public class FallBack: Controller
    {
        public ActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
            "wwwroot", "index.html"), "text/HTML");
        }
    }
}