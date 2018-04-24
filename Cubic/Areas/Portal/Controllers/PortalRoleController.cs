using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Cubic.Controllers;
using Cubic.Data.IdentityModel;
using Cubic.Data.ViewModel;
using Cubic.Repository;
using Cubic.Repository.CoreRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cubic.Areas.Portal.Controllers
{
    [Authorize(Roles = "PortalAdmin")]
    public class PortalRoleController : BaseController
    {
        private readonly IRepositoryQuery<RolePermission, long> _rolePermissionyQuery;
        private readonly IRepositoryCommand<RolePermission, long> _rolePermissionCommand;
        private readonly IActivityLogRepositoryCommand _activityRepo;
        private readonly ILogger _log;
        private readonly IMapper _mapper;

        public PortalRoleController(IActivityLogRepositoryCommand activityRepo,
            IRepositoryCommand<RolePermission, long> rolePermissionCommand,
            IRepositoryQuery<RolePermission, long> rolePermissionQuery, ILogger<PortalRoleController> log,
            IMapper mapper)
        {
            _rolePermissionCommand = rolePermissionCommand;
            _rolePermissionyQuery = rolePermissionQuery;
            _activityRepo = activityRepo;
            _log = log;
            _mapper = mapper;
        }

        public ActionResult Index()
        {
            try
            {
                _log.LogInformation("<<< In Portal Role Page >>>");
                if (TempData["MESSAGE"] != null)
                {
                    ViewBag.Msg = TempData["MESSAGE"] as string;
                }
                //RoleManager
                var result = _mapper.Map<List<ApplicationRoleViewModel>>(_rolePermissionyQuery.GetAllList());
                //var model = result.Select(e => new ApplicationRoleViewModel()
                //{
                //    Id = e.Id,
                //    Name = e.Name
                //});
                return View(result);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return View("Error");
            }

        }

        // GET: ApplicationRoles/Create
        public ActionResult Create()
        {
            CreateViewBagParams();
            return PartialView("_PartialAddEdit", new ApplicationRoleViewModel { Permissions = _utility.GetPermissions() });
        }

        [HttpPost]
        public ActionResult Create(ApplicationRoleViewModel model)
        {
            try
            {
                CreateViewBagParams();
                if (ModelState.IsValid)
                {
                    ApplicationRole applicationRole = new ApplicationRole
                    {
                        Name = model.Name,
                        NormalizedName= model.Name.Trim().ToUpper()

                    };
                    var roleResult = RoleManager.Create(applicationRole);
                    if (!roleResult.Succeeded)
                    {
                        ModelState.AddModelError("", roleResult.Errors.First());
                        return PartialView("_PartialAddEdit", new ApplicationRoleViewModel());
                    }
                    else
                    {
                        _activityRepo.CreateActivityLog(string.Format("Created Portal Role with Name:{0}", applicationRole.Name), this.GetContollerName(), this.GetContollerName(), User.Identity.GetUserId<Int64>(), applicationRole);

                        _rolePermissionyQuery.ExecuteStoreprocedure("DeletePermissionByRoleID @RoleId", new SqlParameter("RoleId", applicationRole.Id));
                        if (model.SelectedPermissionId != null && model.SelectedPermissionId.Any())
                        {
                            foreach (var permissionId in model.SelectedPermissionId)
                            {
                                _rolePermissionCommand.Insert(new RolePermission()
                                {
                                    RoleId = applicationRole.Id,
                                    PermissionId = permissionId,
                                });
                                _rolePermissionCommand.SaveChanges();
                               
                            }
                        }
                      

                    }

                    TempData["MESSAGE"] = "Portal Role " + applicationRole.Name + " was successfully created";
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
                    return PartialView("_PartialAddEdit", model);
                }
            }
            catch (Exception exp)
            {
                _log.Error(exp);
                return View("Error");
            }

        }

        // GET: ApplicationRoles/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                EditViewBagParams();
                if (id <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ApplicationRole applicationRole = await RoleManager.FindByIdAsync(id);
                if (applicationRole == null)
                {
                    return HttpNotFound();
                }
                ApplicationRoleViewModel applicationRoleViewModel = _mapper.Map<ApplicationRoleViewModel>(applicationRole);
                applicationRoleViewModel.Permissions = _utility.GetPermissions();
                applicationRoleViewModel.SelectedPermissionId = _rolePermissionyQuery.GetAllList(c => c.RoleId == id).Select(c => c.PermissionId).ToList();

                //old code
                //ApplicationRole applicationRole = await RoleManager.FindByIdAsync(id);
                //if (applicationRole == null)
                //{
                //    return HttpNotFound();
                //}
                //ApplicationRoleViewModel applicationRoleViewModel = new ApplicationRoleViewModel
                //{
                //    Id = applicationRole.Id,
                //    Name = applicationRole.Name,
                //    Permissions = _utility.GetPermissions(),
                //    SelectedPermissionId = _rolePermissionyQuery.GetAllList(c => c.RoleId == id).Select(c => c.PermissionId).ToList(),
                //};
                return PartialView("_PartialAddEdit", applicationRoleViewModel);
            }
            catch (Exception exp)
            {
                _log.Error(exp);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, ApplicationRoleViewModel applicationRoleViewModel)
        {
            try
            {
                EditViewBagParams();
                if (id <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (ModelState.IsValid)
                {
                    ApplicationRole applicationRole = await RoleManager.FindByIdAsync(applicationRoleViewModel.Id);
                    string originalName = applicationRole.Name;

                    if (originalName == "PortalAdmin" && applicationRoleViewModel.Name != "PortalAdmin")
                    {
                        ModelState.AddModelError("", "You cannot change the name of the Portal Admin role.");
                        return PartialView("_PartialAddEdit", applicationRoleViewModel);
                    }

                    if (originalName != "PortalAdmin" && applicationRoleViewModel.Name == "PortalAdmin")
                    {
                        ModelState.AddModelError("", "You cannot change the name of a role to Portal Admin.");
                        return PartialView("_PartialAddEdit", applicationRoleViewModel);
                    }

                    applicationRole.Name = applicationRoleViewModel.Name;
                    await RoleManager.UpdateAsync(applicationRole);

                    _rolePermissionyQuery.ExecuteStoreprocedure("DeletePermissionByRoleID @RoleId", new SqlParameter("RoleId", applicationRole.Id));
                    if (applicationRoleViewModel.SelectedPermissionId != null && applicationRoleViewModel.SelectedPermissionId.Any())
                    {
                        foreach (var permissionId in applicationRoleViewModel.SelectedPermissionId)
                        {
                            _rolePermissionCommand.Insert(new RolePermission()
                            {
                                RoleId = applicationRole.Id,
                                PermissionId = permissionId,
                            });
                            _rolePermissionCommand.SaveChanges();
                        }
                    }
                    _activityRepo.CreateActivityLog(string.Format("updated Portal Role with Name:{0}", applicationRole.Name), this.GetContollerName(), this.GetContollerName(), User.Identity.GetUserId<Int64>(), applicationRole);

                    TempData["MESSAGE"] = "Portal Role " + applicationRole.Name + " was successfully updated";
                    ModelState.Clear();

                    return Json(new { success = true });
                }else
                {
                    StringBuilder errorMsg = new StringBuilder();

                    foreach (var modelError in ModelState.Values.SelectMany(modelState => modelState.Errors))
                    {
                        errorMsg.AppendLine(modelError.ErrorMessage);
                        ModelState.AddModelError(string.Empty, modelError.ErrorMessage);
                    }
                    ViewBag.ErrMsg = errorMsg.ToString();
                    return PartialView("_PartialAddEdit", applicationRoleViewModel);
                }
            }
            catch (Exception exp)
            {
                _log.Error(exp);
                return View("Error");
            }
           
        }

        // GET: ApplicationRoles/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            ApplicationRole applicationRole = null;
            try
            {
                if (id <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                applicationRole = await RoleManager.FindByIdAsync(id);
                if (applicationRole == null)
                {
                    return HttpNotFound();
                }
                return View(applicationRole);
            }
            catch (Exception exp)
            {
                _log.Error(exp);
                return View("Error");
            }
           
        }

        // POST: ApplicationRoles/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, ApplicationRole applicationRole)
        {
            try
            {
                applicationRole = await RoleManager.FindByIdAsync(id);
                if (applicationRole.Name == "PortalAdmin")
                {
                    ModelState.AddModelError("", "You cannot delete the Admin role.");
                    return RedirectToAction("Index");
                }
                await RoleManager.DeleteAsync(applicationRole);
                TempData["MESSAGE"] = "Portal role " + applicationRole.Name + " was successfully deleted";
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