using AutoMapper;
using Cubic.Controllers;
using Cubic.Data.Entities;
using Cubic.Data.ViewModel;
using Cubic.Repository;
using Cubic.Repository.CoreRepositories;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Cubic.Areas.Portal.Controllers
{
    [Area("Portal")]
    [Authorize(Roles = "PortalAdmin")]
    public class EmailTemplateController : BaseController
    {

        private readonly IRepositoryQuery<EmailTemplate,long> _EmailTemplateQuery;
        private readonly IRepositoryQuery<EmailToken, long> _EmailTokenQuery;
        private readonly IRepositoryCommand<EmailTemplate, long> _EmailTemplateCommand;
        private readonly IActivityLogRepositoryCommand _activityRepo;
        private readonly ILogger _log;
        private readonly IMapper _mapper;

        public EmailTemplateController(IActivityLogRepositoryCommand activityRepo, 
            IRepositoryQuery<EmailToken, long> EmailTokenQuery, 
            IRepositoryQuery<EmailTemplate, long> EmailTemplateQuery, 
            IRepositoryCommand<EmailTemplate, long> EmailTemplateCommand, 
            ILogger<EmailTemplateController> log, IMapper mapper)
        {

            _EmailTemplateQuery = EmailTemplateQuery;
            _EmailTemplateCommand = EmailTemplateCommand;
            _EmailTokenQuery = EmailTokenQuery;
            _activityRepo = activityRepo;
            _log = log;
            _mapper = mapper;
        }
        // GET: APPPortal/EmailTemplate

        public ActionResult Index()
        {
            try
            {
                if (TempData["MESSAGE"] != null)
                {
                    ViewBag.Msg = TempData["MESSAGE"] as string;
                }

                var model = _mapper.Map<List<EmailListViewModel>>(_EmailTemplateQuery.GetAll());
                return View(model);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return View("Error");
            }
        }


        public IEnumerable<SelectListItem> GetEmailTemplateToken(string code)
        {

            var types = _EmailTokenQuery.GetAllList(c => c.EmailCode == code).Select(x =>
                                new SelectListItem
                                {
                                    Value = x.Token.ToString(),
                                    Text = x.Token
                                });

            return new SelectList(types, "Value", "Text");
        }

        public async Task<ActionResult> Edit(int id)
        {
            EditViewBagParams();

            if (id <= 0)
            {
                return new BadRequestResult();
            }
            EmailTemplate emailTemplate = await _EmailTemplateQuery.GetAsync(id);
            if (emailTemplate == null)
            {
                return NotFound($"Unable to load permission with ID '{id}'.");
            }
            var emailTokenModel = _EmailTokenQuery.GetAllList(c => c.EmailCode == emailTemplate.Code).ToList();
            try
            {
                var model = new EmailViewModel()
                {
                    EmailID = emailTemplate.Id,
                    EmailSubject = emailTemplate.Name,
                    EmailCode = emailTemplate.Code,
                    EmailText = emailTemplate.Body,
                    EmailToken = emailTokenModel

                };
                return PartialView("_PartialAddEdit", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                _log.LogError(ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(int id, EmailViewModel systememail)
        {
            try
            {
               
                if (id <= 0)
                {
                    return new BadRequestResult();
                }
                
                if (ModelState.IsValid)
                {
                    EmailTemplate emailTemplate = await _EmailTemplateQuery.GetAsync(id);
                    if (emailTemplate == null)
                    {
                        return NotFound($"Unable to load permission with ID '{id}'.");
                    }
                    emailTemplate.Body = systememail.EmailText;
                    await _EmailTemplateCommand.UpdateAsync(emailTemplate);
                    await _EmailTemplateCommand.SaveChangesAsync();
                    TempData["MESSAGE"] = "Email Template " + emailTemplate.Name + " was successfully updated";
                    ModelState.Clear();
                    return Json(new { success = true });
                }
                else
                {
                    foreach (var modelError in ModelState.Values.SelectMany(modelState => modelState.Errors))
                    {
                        ModelState.AddModelError(string.Empty, modelError.ErrorMessage);
                    }
                    return PartialView("_PartialAddEdit", systememail);
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                _log.LogInformation(ex.Message);
                return View("Error");
            }
        }


    }
}