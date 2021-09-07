using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestApplication.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        public ViewResult NotFound(string message)
        {
            ViewBag.Message = message;
            return View();
        }

        public ActionResult General(string message)
        {
            ViewBag.Message = message;
            return View();
        }
        
    }
}