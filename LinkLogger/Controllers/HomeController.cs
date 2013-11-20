using System.Threading.Tasks;
using System.Web.Mvc;

namespace LinkLogger.Controllers
{
    [Authorize()]
    public class HomeController : Controller
    {
        [RoleFilter("LinkViewer")]
        public ViewResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult AccessDenied(string id)
        {
            ViewBag.MissingRoleName = id;
            return View();
        }
    }
}