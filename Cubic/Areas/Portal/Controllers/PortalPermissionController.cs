using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using log4net;
using System.Data.SqlClient;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Cubic.Controllers;
using Cubic.Repository.CoreRepositories;
using Cubic.Data.IdentityModel;
using Cubic.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Cubic.Data.ViewModel;

namespace Cubic.Areas.Portal.Controllers
{
    [Area("Portal")]
    [Authorize(Roles = "PortalAdmin")]
    public class PortalPermissionController : BaseController
    {
        private readonly IRepositoryQuery<Permission,long> _permissionyQuery;
        private readonly IRepositoryCommand<Permission,long> _permissionCommand;
        private readonly IActivityLogRepositoryCommand _activityRepo;
        private readonly ILogger _log;
        private readonly IMapper _mapper;

        public PortalPermissionController(IActivityLogRepositoryCommand activityRepo, IRepositoryCommand<Permission, long> permissionCommand,
            IRepositoryQuery<Permission, long> permissionQuery,
            ILogger<PortalPermissionController> log, IMapper mapper)
        {
            _permissionCommand = permissionCommand;
            _permissionyQuery = permissionQuery;
            _activityRepo = activityRepo;
            _log = log;
            _mapper = mapper;
        }


        public ActionResult Index()
        {
            if (TempData["MESSAGE"] != null)
            {
                ViewBag.Msg = TempData["MESSAGE"] as string;
            }
            var model = _mapper.Map<List<PermissionViewModel>>(_permissionyQuery.GetAll());
            return View(model);
        }

        public ActionResult Create()
        {
            CreateViewBagParams();
            return PartialView("_PartialAddEdit");
        }

        // POST: Class/Create
        [HttpPost]
        public async  Task<ActionResult> Create(PermissionViewModel permissionVm)
        {
            long currentUserId = GetCurrentUserId();
            try
            {
                CreateViewBagParams();
                if (ModelState.IsValid)
                {
                   
                    var permission = _mapper.Map<PermissionViewModel, Permission>(permissionVm);
                    permission.CreatedBy = currentUserId;
                    await _permissionCommand.InsertAsync(permission);
                    await _permissionCommand.SaveChangesAsync();
                    _activityRepo.CreateActivityLog(string.Format("Created Portal permission with Name:{0}", permission.Name), this.ControllerContext.ActionDescriptor.ControllerName, this.ControllerContext.ActionDescriptor.ActionName, currentUserId, permission);

                    TempData["MESSAGE"] = "Permission " + permissionVm.PermissionName + " was successfully created";
                    ModelState.Clear();
                    return Json(new { success = true });
                }
                else
                {
                    StringBuilder errorMsg = new StringBuilder();

                    foreach (var modelError in ModelState.Values.SelectMany(modelState => modelState.Errors))
                    {
                        errorMsg.AppendLine(modelError.ErrorMessage);
                        ModelState.AddModelError(string.Empty, modelError.ErrorMessage);
                    }
                    ViewBag.ErrMsg = errorMsg.ToString();
                    return PartialView("_PartialAddEdit", permissionVm);
                }

            }
            catch (Exception exp)
            {
                _log.LogError(exp.Message);
                return View("Error");
            }

            
        }

        // GET: Class/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            EditViewBagParams();
            try
            {
                if (id <= 0)
                {
                    return new BadRequestResult();
                }
                var permission = await _permissionyQuery.GetAsync(id);
                if (permission == null)
                {
                    return NotFound($"Unable to load permission with ID '{id}'.");
                }
                var permissionVm = _mapper.Map<PermissionViewModel>(permission);

                return PartialView("_PartialAddEdit", permissionVm);
            }
            catch (Exception exp)
            {
                _log.LogError(exp.Message);
                return View("Error");
            }
            
        }

        // POST: Class/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, PermissionViewModel permissionVm)
        {
            long currentUserId = GetCurrentUserId();
            try
            {
                if (id <= 0)
                {
                    return new BadRequestResult();
                }
                if (ModelState.IsValid)
                {

                    var permissionmodel = await _permissionyQuery.GetAsync(id);
                    if(permissionmodel!= null)
                    {
                        _mapper.Map<PermissionViewModel, Permission>(permissionVm, permissionmodel);
                        permissionmodel.UpdatedBy = currentUserId;
                    }
                    await _permissionCommand.UpdateAsync(permissionmodel);
                    await _permissionCommand.SaveChangesAsync();
                    _activityRepo.CreateActivityLog(string.Format("update Portal permission with Name:{0}", permissionmodel.Name), this.ControllerContext.ActionDescriptor.ControllerName, this.ControllerContext.ActionDescriptor.ActionName,currentUserId, permissionmodel);

                    TempData["MESSAGE"] = "Permission " + permissionVm.PermissionName + " was successfully updated";
                    ModelState.Clear();
                    return Json(new { success = true });
                }
                else
                {

                    StringBuilder errorMsg = new StringBuilder();

                    foreach (var modelError in ModelState.Values.SelectMany(modelState => modelState.Errors))
                    {
                        errorMsg.AppendLine(modelError.ErrorMessage);
                        ModelState.AddModelError(string.Empty, modelError.ErrorMessage);
                    }
                    ViewBag.ErrMsg = errorMsg.ToString();
                    return PartialView("_PartialAddEdit", permissionVm);
                }

            }
            catch (Exception exp)
            {
                _log.LogError(exp.Message);
                return View("Error");
            }


        }

        public async Task<ActionResult> Delete(int id)
        {
            Permission permission = null;
            try
            {

                if (id <= 0)
                {
                    return new BadRequestResult();
                }
                permission = await _permissionyQuery.GetAsync(id);
                if (permission == null)
                {
                    return NotFound($"Unable to load permission with ID '{id}'.");
                } 
                return View(permission);

            }
            catch (Exception exp)
            {
                _log.LogError(exp.Message);
                return View("Error");
            }

           
        }

        [HttpPost]
        public ActionResult Delete(int id, Permission permission)
        {
            try
            {
                _permissionCommand.Delete(id);
                _permissionCommand.Save();
                TempData["MESSAGE"] = "Permission " + permission.Name + " was successfully deleted";
                ModelState.Clear();
                return RedirectToAction("Index");

            }
            catch (Exception exp)
            {
                _log.LogError(exp.Message);
                return View("Error");
            }
        }
    }
}