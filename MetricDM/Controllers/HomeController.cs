using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MetricDM.Controllers
{
    //This is a test
    //This is another test
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.AlexMessage = "Alex was here...";
            return View();
        }
    }
}