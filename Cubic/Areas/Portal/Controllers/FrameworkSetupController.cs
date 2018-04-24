using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Cubic.Controllers;
using Cubic.Data.Entities;
using Cubic.Data.IdentityModel;
using Cubic.Data.ViewModel;
using Cubic.Repository;
using Cubic.Repository.CoreRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Cubic.Areas.Portal.Controllers
{
    [Area("Portal")]
   // [Route("Portal/[controller]")]
    public class FrameworkSetupController : Controller
    {
        private FrameworkSetupViewModel _setupContract;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepositoryCommand<ApplicationUserPasswordHistory, long> _applicationUserPwdhistoryCommand;
        private readonly IRepositoryQuery<Application, int> _applicationQuery;
        private readonly IRepositoryCommand<Application, int> _applicationCommand;
        private readonly IRepositoryQuery<ApplicationUser, long> _applicationUserQuery;
        private readonly IRepositoryQuery<ApplicationRole, long> _applicationRoleQuery;
        private readonly IRepositoryQuery<PortalVersion, int> _portalversionQuery;
        private readonly IActivityLogRepositoryCommand _activityRepo;
        private readonly IMapper _mapper;
        private readonly ILogger _log;

        public FrameworkSetupController(ILogger<FrameworkConfigController> log,UserManager<ApplicationUser> userManager,IActivityLogRepositoryCommand activityRepo, IMapper mapper, IRepositoryQuery<Application, int> application, IRepositoryCommand<Application, int> applicationCommand, IRepositoryQuery<PortalVersion, int> portalversion, IRepositoryQuery<ApplicationUser, long> applicationUser, IRepositoryCommand<ApplicationUserPasswordHistory, long> applicationUserPwdhistory, IRepositoryQuery<ApplicationRole, long> applicationRoleQuery)
        {
            _userManager = userManager;
            _applicationQuery = application;
            _applicationCommand = applicationCommand;
            _applicationUserQuery = applicationUser;
            _applicationUserPwdhistoryCommand = applicationUserPwdhistory;
            _portalversionQuery = portalversion;
            _activityRepo = activityRepo;
            _mapper = mapper;
            _log = log;
            _applicationRoleQuery = applicationRoleQuery;
            _setupContract = new FrameworkSetupViewModel();
        }

       
        [AllowAnonymous]
        public ActionResult Start(string nextButton)
        {
            if (nextButton != null) return RedirectToAction("CurrentConfig");
            ViewBag.WelcomeMessage = "Welcome to Cubic ASP.Net Core Framework APP";
            return View(_setupContract);

        }

        [AllowAnonymous]
        public ActionResult CurrentConfig(string nextButton, string backButton)
        {

          //  _activityRepo.CreateActivityLog("In Framework setup currentconfig", this.GetContollerName(), this.GetContollerName(), 0, null);

            if (nextButton != null)
            {
                return RedirectToAction("FrameworkSetting");
            }
            if (backButton != null)
            {
                return RedirectToAction("Start");
            }
            if (!LoadDefaultSettings())
            {
                return View(_setupContract);
            }

            return View(_setupContract);

        }

        [AllowAnonymous]
        public ActionResult FrameworkSetting(FrameworkSetupViewModel model,string nextButton, string backButton)
        {
            ModelState.Clear();
           _activityRepo.CreateActivityLog("In Framework setting currentconfig", this.ControllerContext.ActionDescriptor.ControllerName, this.ControllerContext.ActionDescriptor.ActionName, 0, null);
            if (backButton != null)
            {
                return RedirectToAction("CurrentConfig");
            }

            if (nextButton != null)
            {
                if (!ModelState.IsValid)
                {
                    return View(_setupContract);
                }

                if (string.IsNullOrEmpty(model.PortalSetting.PortalTitle))
                {
                    ModelState.AddModelError("", "Portal title is required");
                    return View(_setupContract);
                }

                var app = new Application { ApplicationName = model.PortalSetting.PortalTitle, Description = model.PortalSetting.PortalDescription, TermsAndConditions = model.PortalSetting.TermsAndConditionPath, HasAdminUserConfigured = false };
                if (_applicationQuery.GetAll().Any())
                {
                    Application datamodel = _applicationQuery.GetAll().FirstOrDefault();
                    app.Id = datamodel.Id;
                    datamodel.ApplicationName = app.ApplicationName;
                    datamodel.Description = app.Description;
                    datamodel.TermsAndConditions = app.TermsAndConditions;
                    _applicationCommand.Update(datamodel);
                }
                else
                {
                    _applicationCommand.Insert(app);

                }
                _applicationCommand.SaveChanges();

                if (app.Id >= 1)
                {
                    _activityRepo.CreateActivityLog("creating Framework application data", this.ControllerContext.ActionDescriptor.ControllerName, this.ControllerContext.ActionDescriptor.ActionName, 0, app);
                    return RedirectToAction("FramewokAdmin");
                }
                ModelState.AddModelError("", "Unable to save framework settings due to internal error! Please try again later");
                return View(_setupContract);
            }
            var application = _applicationQuery.GetAll().FirstOrDefault();
            var portalInfo = _mapper.Map<PortalSettingViewModel>(application);
            if (portalInfo == null)
            {
                // ModelState.AddModelError("", "Unable to initialize portal information due to internal error! Please try again later");
                return View(_setupContract);
            }

            _setupContract.PortalSetting = portalInfo;
            return View(_setupContract);

            //add settings to DB
        }

        [AllowAnonymous]
        public async Task<ActionResult> FramewokAdmin(FrameworkSetupViewModel model,string nextButton, string backButton)
        {
            string msg;
            if (backButton != null)
            {
                return RedirectToAction("FrameworkSetting");
            }

            if (nextButton != null)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                if (string.Compare(model.AdminUserSetting.Password,
                    model.AdminUserSetting.ConfirmPassword, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    ViewBag.ErrMsg = "Password and confirm password must be equal";
                    // ModelState.AddModelError("","Password and confirm password must be equal");
                    return View(model);
                }

                var roleIndb = _applicationRoleQuery.GetAllList();
                if (_userManager.Users.ToList().Any())
                {
                    var adminusermodel = _userManager.Users.ToList().FirstOrDefault();
                    var tokencode = await _userManager.GeneratePasswordResetTokenAsync(adminusermodel);
                    var result = await _userManager.ResetPasswordAsync(adminusermodel, tokencode, model.AdminUserSetting.Password);
                    if (result.Succeeded)
                    {

                        ApplicationUserPasswordHistory passwordModel = new ApplicationUserPasswordHistory();
                        passwordModel.UserId = adminusermodel.Id;
                        passwordModel.DateCreated = DateTime.Now;
                        passwordModel.HashPassword = "";
                        //ExtensionUtility.Encrypt(model.AdminUserSetting.Password);
                        passwordModel.CreatedBy = adminusermodel.Id;
                        _applicationUserPwdhistoryCommand.Insert(passwordModel);
                        _applicationUserPwdhistoryCommand.Save();

                        var addRoleResult = await _userManager.AddToRoleAsync(adminusermodel, "PortalAdmin");
                        if (addRoleResult.Succeeded)
                        {
                            Application applicationmodel = _applicationQuery.GetAll().FirstOrDefault();
                            applicationmodel.HasAdminUserConfigured = true;
                            _applicationCommand.Update(applicationmodel);
                            _applicationCommand.SaveChanges();
                            return RedirectToAction("Login", "Account");
                        }



                    }
                    else
                    {
                        ModelState.AddModelError("", result.Errors.FirstOrDefault().ToString());
                    }
                }
                else
                {
                    var usermodel = new ApplicationUser
                    {
                        FirstName = model.AdminUserSetting.FirstName,
                        LastName = model.AdminUserSetting.LastName,
                        MiddleName = model.AdminUserSetting.MiddleName,
                        UserName = model.AdminUserSetting.UserName,
                        Email = model.AdminUserSetting.Email,
                        MobileNumber = model.AdminUserSetting.MobileNumber,
                        PhoneNumber = model.AdminUserSetting.PhoneNumber,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        TwoFactorEnabled = false,
                        LockoutEnabled = false,
                        AccessFailedCount = 0,
                        DateCreated = DateTime.Now,
                        IsFirstLogin = false
                    };
                    var result = await _userManager.CreateAsync(usermodel, model.AdminUserSetting.Password);
                    if (result.Succeeded)
                    {

                        ApplicationUserPasswordHistory passwordModel = new ApplicationUserPasswordHistory();
                        passwordModel.UserId = usermodel.Id;
                        passwordModel.DateCreated = DateTime.Now;
                        passwordModel.HashPassword = "";
                        //ExtensionUtility.Encrypt(model.AdminUserSetting.Password);
                        passwordModel.CreatedBy = usermodel.Id;
                        _applicationUserPwdhistoryCommand.Insert(passwordModel);
                        _applicationUserPwdhistoryCommand.Save();

                        var addRoleResult = await _userManager.AddToRoleAsync(usermodel, "PortalAdmin");
                        if (addRoleResult.Succeeded)
                        {
                            Application applicationmodel = _applicationQuery.GetAll().FirstOrDefault();
                            applicationmodel.HasAdminUserConfigured = true;
                            _applicationCommand.Update(applicationmodel);
                            _applicationCommand.SaveChanges();
                            return RedirectToAction("Login", "Account");
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("", result.Errors.FirstOrDefault().ToString());
                    }
                }
               
                return View(_setupContract);
            }

            var user = _userManager.Users.ToList().FirstOrDefault();
            var userInfo = _mapper.Map<AdminUserSettingViewModel>(user);
            if (userInfo == null)
            {
                //ModelState.AddModelError("", "Unable to initialize admin user information due to internal error! Please try again later");
                return View(_setupContract);
            }
            _setupContract.AdminUserSetting = userInfo;
            return View(_setupContract);

        }

        #region Controller Helpers

        private bool LoadDefaultSettings()
        {
            var portalversion= _portalversionQuery.GetAll().FirstOrDefault();
            var portalversionmodel = _mapper.Map<FrameworkDefaultSettingViewModel>(portalversion);
            if (portalversionmodel == null)
            {
                // ModelState.AddModelError("", "Unable to load default settings! Please check config file");
                return false;
            }
            if (_setupContract == null)
            {
                _setupContract = new FrameworkSetupViewModel();
            }
            _setupContract.DefaultSetting = portalversionmodel;
            return true;
        }
        #endregion
    }
}