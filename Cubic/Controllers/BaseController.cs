using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cubic.Data.IdentityModel;
using Cubic.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cubic.Controllers
{
    public class BaseController : Controller
    {

        protected  UserManager<ApplicationUser> _userManager;
        protected  SignInManager<ApplicationUser> _signInManager;
        protected RoleManager<ApplicationRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _contextAccessor;

        public BaseController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IEmailSender emailSender,
            ILogger<BaseController> logger, IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _logger = logger;
            _contextAccessor = contextAccessor;
        }
        
        //public Controller(IHttpContextAccessor httpContextAccessor)
        //{
        //    var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value
        //}
        public BaseController()
        {
        }
        
        protected void CreateViewBagParams()
        {
            ViewBag.IsUpdate = false;
            ViewBag.ModalTitle = "Add";
            ViewBag.ButtonAction = "Save";
            ViewBag.PostAction = "Create";
            ViewBag.ImageProperty = "fileinput-new";
            ViewBag.ImageProperty2 = "fileinput-new";
            ViewBag.ButtonActionCss = "btn btn-primary";
            ViewBag.ButtonActionAddIcon = "fa fa-plus-circle";
            ViewBag.ButtonActionCloseIcon = "fa fa-plus-circle";
        }
        protected void EditViewBagParams()
        {
            ViewBag.IsUpdate = true;
            ViewBag.ModalTitle = "Edit";
            ViewBag.ButtonAction = "Update";
            ViewBag.PostAction = "Edit";
        }
        protected void EditViewBagParams(string imageUrl)
        {
            ViewBag.IsUpdate = true;
            ViewBag.ModalTitle = "Edit";
            ViewBag.ButtonAction = "Update";
            ViewBag.PostAction = "Edit";
            ViewBag.ImageProperty = string.IsNullOrEmpty(imageUrl) ? "fileinput-new" : "fileinput-exists";
            ViewBag.ImageProperty2 = "fileinput-exists";
        }
        
        public long  GetCurrentUserId()
        {
            return long.Parse(_userManager.GetUserId(User));
            //var ident = User.Identity as ClaimsIdentity;
            //var userID = ident.Claims.FirstOrDefault(c => c.Type == idClaimType)?.Value;

            //ClaimsPrincipal currentUser = this.User;
            //var currentUserID = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            //return long.Parse(currentUserID);
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        protected string GetUserName()
        {
            string userName = string.Empty;
            if (_contextAccessor.HttpContext.User != null)
            {
                var identity = _contextAccessor.HttpContext.User.Identity;

                if (identity != null && identity.IsAuthenticated)
                {
                    userName = identity.Name;
                }
            }
            return userName;
        }

        protected long GetLastUserId()
        {
            return _userManager.Users.ToList().Last().Id + 1;
        }

        protected void EditViewBagParams(string imageUrl, string imageUrl2)
        {
            ViewBag.IsUpdate = true;
            ViewBag.ModalTitle = "Edit";
            ViewBag.ButtonAction = "Update";
            ViewBag.PostAction = "Edit";
            ViewBag.ImageProperty = string.IsNullOrEmpty(imageUrl) ? "fileinput-new" : "fileinput-exists";
            ViewBag.ImageProperty2 = string.IsNullOrEmpty(imageUrl2) ? "fileinput-new" : "fileinput-exists";

        }
        //protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        //{
        //    return new JsonNetResult
        //    {
        //        Value = data,
        //        ContentType = contentType,
        //        ContentEncoding = contentEncoding,
        //        JsonRequestBehavior = behavior
        //    };
        //}
       

        protected bool IsUserPortalAdmin()
        {
            return false;
        }

        
    }
}