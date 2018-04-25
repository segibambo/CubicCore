using Cubic.Data.Entities;
using Cubic.Data.IdentityModel;
using Cubic.Data.ViewModel;
using Cubic.Repository.CoreRepositories;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cubic.Services
{

    public static class RestClientExtensions
    {
        public static async Task<RestResponse> ExecuteAsync(this RestClient client, RestRequest request)
        {
            TaskCompletionSource<IRestResponse> taskCompletion = new TaskCompletionSource<IRestResponse>();
            RestRequestAsyncHandle handle = client.ExecuteAsync(request, r => taskCompletion.SetResult(r));
            return (RestResponse)(await taskCompletion.Task);
        }
    }
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private readonly AppSettings _appSettings;
        private readonly IRepositoryQuery<EmailTemplate, long> _emailTemplateQuery;
        private readonly IRepositoryQuery<EmailToken, long> _emailTokenQuery;
        private readonly IRepositoryCommand<EmailLog, long> _emailLogRepositoryCommand;
        //_emailLogRepositoryCommand
        public EmailSender(IOptions<AppSettings> settings, IRepositoryQuery<EmailTemplate, long> emailTemplateQuery, IRepositoryQuery<EmailToken, long> emailTokenQuery, IRepositoryCommand<EmailLog, long> emailLogRepositoryCommand)
        {
            _appSettings = settings.Value;
            _emailTemplateQuery = emailTemplateQuery;
            _emailTokenQuery = emailTokenQuery;
            _emailLogRepositoryCommand = emailLogRepositoryCommand;
        }
        

        public Task SendEmailAsync(string email, string subject, string message)
        {
            //_appSettings

            var client = new RestClient();
            client.BaseUrl = new Uri(_appSettings.EmailBaseUrl);
            // client.BaseUrl = new Uri("https://api.elasticemail.com");
            var request = new RestRequest(_appSettings.EmailServiceUrl, Method.POST);
            //request.AddParameter("apikey", "21eddf0a-7467-4b23-a2c0-6134e1bc60f2");
            //request.AddParameter("from", "chamsservices@gmail.com");

            request.AddParameter("apikey", _appSettings.EmailApiKey);
            request.AddParameter("from", _appSettings.EmailFromAddress);

            request.AddParameter("to", email);
            request.AddParameter("subject", subject);
            request.AddParameter("body", message);
            request.AddParameter("isTransactional", true.ToString());

            //method 1
           // var respon = client.ExecuteAsync(request);

            //method 2
            //var response = new RestResponse();
            //Task.Run(async () =>
            //{
            //    response = await GetResponseContentAsync(client, request) as RestResponse;
            //}).Wait();
            //var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);


            //method 3

            TaskCompletionSource<IRestResponse> taskCompletion = new TaskCompletionSource<IRestResponse>();
            RestRequestAsyncHandle handle = client.ExecuteAsync(
                request, r => taskCompletion.SetResult(r));
            RestResponse response1 = (RestResponse)(taskCompletion.Task.Result);
            ElasticEmailResponse  responsemodel= JsonConvert.DeserializeObject<ElasticEmailResponse>(response1.Content);
            if (responsemodel.success)
            {
                var transcationId = responsemodel.data.transactionid;
                var messageId = responsemodel.data.messageid;
            }
            //same with 3 but with async
            //TaskCompletionSource<IRestResponse> taskCompletion = new TaskCompletionSource<IRestResponse>();
            //RestRequestAsyncHandle handle = client.ExecuteAsync(
            //    request, r => taskCompletion.SetResult(r));

            //RestResponse response1 = (RestResponse)(await taskCompletion.Task);
            //return JsonConvert.DeserializeObject<SomeObject>(response.Content);

            return Task.CompletedTask;
        }


       

        public Task SendEmailResetPasswordAsync(string emailcode, ApplicationUser usermodel, string resetUrl)
        {
            if (string.IsNullOrEmpty(emailcode))
            {
                List<EmailToken> emailtoken = new List<EmailToken>();
                var emailTemplate = _emailTemplateQuery.FirstOrDefault(m => m.Code == emailcode);
                if (emailTemplate.Id > 0)
                {
                    List<EmailToken> tokenCol = _emailTokenQuery.GetAllList(m => m.EmailCode == emailTemplate.Code).ToList();
                    foreach (var token in tokenCol)
                    {
                        if (token.Token.Equals("{Name}"))
                        {
                            token.PreviewText = usermodel.FirstName + " " + usermodel.LastName;
                        }
                        else if (token.Token.Equals("{Email}"))
                        {
                            token.PreviewText = usermodel.Email ?? string.Empty;
                        }
                        else if (token.Token.Equals("{Url}"))
                        {
                            token.PreviewText = resetUrl;
                        }
                    }
                    
                    EmailLog mlog = new EmailLog();
                    mlog.Receiver = usermodel.Email;
                    mlog.Sender = _appSettings.EmailFromAddress;
                    mlog.Subject = "Password Reset Notification";
                    mlog.MessageBody = GeneratePreviewHTML(emailTemplate.Body, tokenCol);
                    mlog.DateCreated = mlog.DateToSend = DateTime.Now;
                    mlog.IsSent = mlog.HasAttachment = false;
                    var messageresponse=EmailSenderHelper(mlog.Receiver, mlog.Subject,mlog.MessageBody);
                    if (messageresponse.success)
                    {
                        mlog.IsSent = true;
                    }
                    _emailLogRepositoryCommand.Insert(mlog);
                    _emailLogRepositoryCommand.SaveChanges();
                }
            }
            return Task.CompletedTask;
        }

        public ElasticEmailResponse EmailSenderHelper(string email,string subject,string message)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(_appSettings.EmailBaseUrl);
            var request = new RestRequest(_appSettings.EmailServiceUrl, Method.POST);

            request.AddParameter("apikey", _appSettings.EmailApiKey);
            request.AddParameter("from", _appSettings.EmailFromAddress);

            request.AddParameter("to", email);
            request.AddParameter("subject", subject);
            request.AddParameter("body", message);
            request.AddParameter("isTransactional", true.ToString());

            TaskCompletionSource<IRestResponse> taskCompletion = new TaskCompletionSource<IRestResponse>();
            RestRequestAsyncHandle handle = client.ExecuteAsync(
                request, r => taskCompletion.SetResult(r));
            RestResponse response1 = (RestResponse)(taskCompletion.Task.Result);
            ElasticEmailResponse responsemodel = JsonConvert.DeserializeObject<ElasticEmailResponse>(response1.Content);
           
            return responsemodel;
        }
        public static Task<IRestResponse> GetResponseContentAsync(RestClient theClient, RestRequest theRequest)
        {
            var tcs = new TaskCompletionSource<IRestResponse>();
            theClient.ExecuteAsync(theRequest, response => {
                tcs.SetResult(response);
            });
            return tcs.Task;
        }


        public static string GeneratePreviewHTML(string htmlcode, List<EmailToken> lstToken)
        {
            try
            {
                foreach (EmailToken item in lstToken)
                {
                    htmlcode = htmlcode.Replace(item.Token, item.PreviewText);
                }
            }
            catch { }
            return htmlcode;
        }
    }
}
