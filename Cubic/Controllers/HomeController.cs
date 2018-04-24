using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Cubic.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cubic.Controllers
{
    public class HomeController : Controller
    {
        //using appsetting in controller
        //private string _token;

        //public HomeController(IOptions<AppSettings> settings)
        //{
        //    _token = settings.Options.token;
        //}
        public IActionResult Index()
        {
            string controllerName = ControllerContext.ActionDescriptor.ControllerName;
            string ActionName = ControllerContext.ActionDescriptor.ActionName;
            // string controllerName=((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)ViewContext.ActionDescriptor).ActionName;

            return RedirectToAction("Index", "FrameworkConfig");
            //return View();

        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
