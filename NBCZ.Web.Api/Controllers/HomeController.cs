using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBCZ.Web.Api.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Content("hello webapi");
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
