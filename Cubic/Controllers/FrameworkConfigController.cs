using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Cubic.Data.Entities;
using Cubic.Data.IdentityModel;
using Cubic.Data.Models;
using Cubic.Repository;
using Cubic.Repository.CoreRepositories;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cubic.Controllers
{
    public class FrameworkConfigController : Controller
    {
        private readonly IRepositoryQuery<Application, int> _applicationQuery;
        private readonly IRepositoryQuery<ApplicationUser, long> _applicationUserQuery;
        private readonly IActivityLogRepositoryCommand _activityRepo;
        private readonly ILogger _log;
        public FrameworkConfigController(ILogger<FrameworkConfigController> log, IActivityLogRepositoryCommand activityRepo, IRepositoryQuery<Application, int> application, IRepositoryQuery<ApplicationUser, long> applicationUser)
        {
            _applicationQuery = application;
            _applicationUserQuery = applicationUser;
            _activityRepo = activityRepo;
            _log = log;
        }
        // GET: FrameworkConfig
        [AllowAnonymous]
        public ActionResult Index()
        {

            //var controller = ViewContext.RouteData.Values["Controller"];
            //var action = ViewContext.RouteData.Values["Action"];

            try
            {
                _log.LogInformation("Cubic ASP.Net Core Framework Config checked @ : {0}", DateTime.Now);
                _activityRepo.CreateActivityLog("In Framework setting checking if application portal has being configured", this.ControllerContext.ActionDescriptor.ControllerName,  this.ControllerContext.ActionDescriptor.ActionName, 0, null);
                if (_applicationQuery.Count() >= 1)
                {
                    if (_applicationQuery.GetAll().FirstOrDefault().HasAdminUserConfigured)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        return RedirectToAction("Start", "FrameworkSetup", new { area = "Portal" });
                    }

                }
                else
                {
                    return RedirectToAction("Start", "FrameworkSetup", new { area = "Portal" });
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
}