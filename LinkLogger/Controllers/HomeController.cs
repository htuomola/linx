using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LinkLogger.DataAccess;
using LinkLogger.Models;

namespace LinkLogger.Controllers
{
    [Authorize()]
    public class HomeController : Controller
    {
        [RoleFilter("LinkViewer")]
        public async Task<ViewResult> Index()
        {
            Link[] links;
            using (var ctx = new ApplicationDbContext())
            {
                links = await ctx.Links.OrderByDescending(l => l.RegisteredAt).Take(20).ToArrayAsync();
            }

            return View(links);
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