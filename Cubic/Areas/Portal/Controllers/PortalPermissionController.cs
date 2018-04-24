﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Text;
using SleekSoftMVCFramework.Utilities;
using SleekSoftMVCFramework.Data.IdentityModel;
using SleekSoftMVCFramework.Data.IdentityService;
using SleekSoftMVCFramework.Repository;
using SleekSoftMVCFramework.Repository.CoreRepositories;
using SleekSoftMVCFramework.Data.ViewModel;
using SleekSoftMVCFramework.Controllers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Net;
using log4net;
using System.Data.SqlClient;
using AutoMapper;

namespace Cubic.Areas.Portal.Controllers
{
    [Authorize(Roles = "PortalAdmin")]
    public class PortalPermissionController : BaseController
    {
        private readonly IRepositoryQuery<Permission,long> _permissionyQuery;
        private readonly IRepositoryCommand<Permission,long> _permissionCommand;
        private readonly IActivityLogRepositoryCommand _activityRepo;
        private readonly ILog _log;
        private readonly IMapper _mapper;

        public PortalPermissionController(IActivityLogRepositoryCommand activityRepo, IRepositoryCommand<Permission, long> permissionCommand,
            IRepositoryQuery<Permission, long> permissionQuery, 
            ILog log, IMapper mapper)
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
            //var model = _permissionyQuery.GetAll().Select(e => new PermissionViewModel()
            //{
            //    PermissionName = e.Name,
            //    PermissionId = e.Id,
            //    PermissionCode=e.Code

            //});

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
            try
            {
                CreateViewBagParams();
                if (ModelState.IsValid)
                {

                    var permission = _mapper.Map<PermissionViewModel, Permission>(permissionVm);
                    permission.CreatedBy = User.Identity.GetUserId<Int64>();
                    //var permission = new Permission()
                    //{
                    //    Name = permissionVm.PermissionName,
                    //    Code=permissionVm.PermissionCode,
                    //    CreatedBy=User.Identity.GetUserId<Int64>()
                    //};
                    await _permissionCommand.InsertAsync(permission);
                    await _permissionCommand.SaveChangesAsync();
                    _activityRepo.CreateActivityLog(string.Format("Created Portal permission with Name:{0}", permission.Name), this.GetContollerName(), this.GetContollerName(), User.Identity.GetUserId<Int64>(), permission);

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
                _log.Error(exp);
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
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }


                var permission = await _permissionyQuery.GetAsync(id);
                if (permission == null)
                {
                    return HttpNotFound();
                }
                var permissionVm = _mapper.Map<PermissionViewModel>(permission);

                //var permission =await  _permissionyQuery.GetAsync(id);
                //if (permission == null)
                //{
                //    return HttpNotFound();
                //}
                //var permissionVm = new PermissionViewModel()
                //{
                //    PermissionName = permission.Name,
                //    PermissionId = permission.Id,
                //    PermissionCode= permission.Code
                //};

                return PartialView("_PartialAddEdit", permissionVm);
            }
            catch (Exception exp)
            {
                _log.Error(exp);
                return View("Error");
            }
            
        }

        // POST: Class/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, PermissionViewModel permissionVm)
        {
            try
            {
                if (id <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (ModelState.IsValid)
                {

                    var permissionmodel = await _permissionyQuery.GetAsync(id);
                    if(permissionmodel!= null)
                    {
                        _mapper.Map<PermissionViewModel, Permission>(permissionVm, permissionmodel);
                        permissionmodel.UpdatedBy = User.Identity.GetUserId<Int64>();
                    }
                    //var permissionmodel = await _permissionyQuery.GetAsync(id);
                    //permissionmodel.Name = permissionVm.PermissionName;
                    //permissionmodel.Code = permissionVm.PermissionCode;

                    await _permissionCommand.UpdateAsync(permissionmodel);
                    await _permissionCommand.SaveChangesAsync();
                    _activityRepo.CreateActivityLog(string.Format("update Portal permission with Name:{0}", permissionmodel.Name), this.GetContollerName(), this.GetContollerName(), User.Identity.GetUserId<Int64>(), permissionmodel);

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
                _log.Error(exp);
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
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                permission = await _permissionyQuery.GetAsync(id);
                if (permission == null)
                {
                    return HttpNotFound();
                }
                return View(permission);

            }
            catch (Exception exp)
            {
                _log.Error(exp);
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
                _log.Error(exp);
                return View("Error");
            }
        }
    }
}