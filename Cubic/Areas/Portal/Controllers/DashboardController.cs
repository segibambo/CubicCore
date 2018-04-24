using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Cubic.Areas.Portal.Controllers
{
    [Area("Portal")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            if (TempData["MESSAGE"] != null)
            {
                ViewBag.Msg = TempData["MESSAGE"] as string;
            }
            return View();
        }
    }
}