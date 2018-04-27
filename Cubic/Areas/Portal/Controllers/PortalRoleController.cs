using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Cubic.Areas.Portal.Controllers
{
    [Area("Portal")]
    [Authorize(Roles = "PortalAdmin")]
    public class PortalRoleController : BaseController
    {
        private readonly IRepositoryQuery<RolePermission, long> _rolePermissionyQuery;
        private readonly IRepositoryQuery<Permission, long> _permissionyQuery;
        private readonly IRepositoryCommand<RolePermission, long> _rolePermissionCommand;
        private readonly IActivityLogRepositoryCommand _activityRepo;
        private readonly ILogger _log;
        private readonly IMapper _mapper;

        public PortalRoleController(IActivityLogRepositoryCommand activityRepo,
            IRepositoryCommand<RolePermission, long> rolePermissionCommand,
            IRepositoryQuery<Permission, long> permissionyQuery,
            IRepositoryQuery<RolePermission, long> rolePermissionQuery,
            ILogger<PortalRoleController> log,
            IMapper mapper)
        {
            _rolePermissionCommand = rolePermissionCommand;
            _rolePermissionyQuery = rolePermissionQuery;
            _permissionyQuery = permissionyQuery;
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
                var result = _mapper.Map<List<ApplicationRoleViewModel>>(_rolePermissionyQuery.GetAllList());
                return View(result);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return View("Error");
            }

        }

        public IEnumerable<SelectListItem> GetPermission()
        {

            var types = _permissionyQuery.GetAll().Select(x =>
                                new SelectListItem
                                {
                                    Value = x.Id.ToString(),
                                    Text = x.Name
                                });

            return new SelectList(types, "Value", "Text");
        }


        // GET: ApplicationRoles/Create
        public ActionResult Create()
        {
            CreateViewBagParams();
            return PartialView("_PartialAddEdit", new ApplicationRoleViewModel { Permissions = GetPermission() });
        }

        [HttpPost]
        public async Task<ActionResult> Create(ApplicationRoleViewModel model)
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
                    var roleResult =await  _roleManager.CreateAsync(applicationRole);
                    if (!roleResult.Succeeded)
                    {
                        ModelState.AddModelError("", roleResult.Errors.First().ToString());
                        return PartialView("_PartialAddEdit", new ApplicationRoleViewModel());
                    }
                    else
                    {
                        _activityRepo.CreateActivityLog(string.Format("Created Portal Role with Name:{0}", applicationRole.Name), this.ControllerContext.ActionDescriptor.ControllerName, this.ControllerContext.ActionDescriptor.ActionName, GetCurrentUserId(), applicationRole);

                        _rolePermissionyQuery.ExecuteStoreprocedure("spDeletePermissionByRoleID @RoleId", new SqlParameter("RoleId", applicationRole.Id));
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
                _log.LogError(exp.Message);
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
                    return new BadRequestResult();
                }
                ApplicationRole applicationRole = await _roleManager.FindByIdAsync(id.ToString());
                if (applicationRole == null)
                {
                    return NotFound($"Unable to load Role with ID '{id}'.");
                }
                
                ApplicationRoleViewModel applicationRoleViewModel = _mapper.Map<ApplicationRoleViewModel>(applicationRole);
                applicationRoleViewModel.Permissions = GetPermission();
                applicationRoleViewModel.SelectedPermissionId = _rolePermissionyQuery.GetAllList(c => c.RoleId == id).Select(c => c.PermissionId).ToList();
                return PartialView("_PartialAddEdit", applicationRoleViewModel);
            }
            catch (Exception exp)
            {
                _log.LogError(exp.Message);
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
                    return new BadRequestResult();
                }
                ApplicationRole applicationRole = await _roleManager.FindByIdAsync(id.ToString());
                if (applicationRole == null)
                {
                    return NotFound($"Unable to load Role with ID '{id}'.");
                }
                if (ModelState.IsValid)
                {
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
                    await _roleManager.UpdateAsync(applicationRole);

                    _rolePermissionyQuery.ExecuteStoreprocedure("spDeletePermissionByRoleID @RoleId", new SqlParameter("RoleId", applicationRole.Id));
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
                    _activityRepo.CreateActivityLog(string.Format("updated Portal Role with Name:{0}", applicationRole.Name), this.ControllerContext.ActionDescriptor.ControllerName, this.ControllerContext.ActionDescriptor.ActionName,GetCurrentUserId(), applicationRole);

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
                _log.LogError(exp.Message);
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
                    return new BadRequestResult();
                }
                applicationRole = await _roleManager.FindByIdAsync(id.ToString());
                if (applicationRole == null)
                {
                    return NotFound($"Unable to load Role with ID '{id}'.");
                }
                return View(applicationRole);
            }
            catch (Exception exp)
            {
                _log.LogError(exp.Message);
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
                applicationRole = await _roleManager.FindByIdAsync(id.ToString());
                if (applicationRole.Name == "PortalAdmin")
                {
                    ModelState.AddModelError("", "You cannot delete the Admin role.");
                    return RedirectToAction("Index");
                }
                await _roleManager.DeleteAsync(applicationRole);
                TempData["MESSAGE"] = "Portal role " + applicationRole.Name + " was successfully deleted";
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