using Sabio.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sabio.Models.Requests;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.Extensions.Options;
using Sabio.Models.AppSettings;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Sabio.Services.Interfaces;
using SendGrid.Helpers.Mail.Model;
using Amazon.Runtime.Internal;
using Sabio.Models.Requests.InviteMembers;
using Sabio.Models.Domain.Messages;
using Sabio.Models.Enums;
using Sabio.Models.Domain.Organizations;

namespace Sabio.Services
{
    public class EmailsService : IEmailsService
    {



        private AppKeys _appKeys;
        private readonly IWebHostEnvironment _webHostEnvironment;
         IDataProvider _data = null;
        private IUserService _userService = null;

        public EmailsService(IDataProvider data, IOptions<AppKeys> appKeys, IWebHostEnvironment webHostEnvironment, IUserService userService)
        {
            _appKeys = appKeys.Value;
            _data = data;
            _webHostEnvironment = webHostEnvironment;
            _userService = userService;
        }

        private async Task SendEmail( SendGridMessage msg) 
        {  
            var client = new SendGridClient(_appKeys.BackUpSendGridAppKey);
            var response = await client.SendEmailAsync(msg);
        }
        private async Task SendPhishing(SendGridMessage notice)
        {
            var client = new SendGridClient(_appKeys.SendGridKey);
            var response = await client.SendEmailAsync(notice);
   
        }

        public async void WelcomeEmail()
        {
           
            var from = new EmailAddress("fakeEmail@dispostable.com", "Example User");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("fakeEmail@dispostable.com", "Example User");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = GetTemplate();
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent );
            await SendEmail(msg);
        }
        public async void ContactUsEmail(ContactUsAddRequest model)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            var from = new EmailAddress(model.From);
            var subject = model.Subject;
            var to = new EmailAddress(_appKeys.AccountEmail);
            var plainTextContent = "";
            string path = Path.Combine(webRootPath, "EmailTemplates", "ContactUsReply.html");
            var htmlContent = File.ReadAllText(path).Replace("{{message}}", model.Message)
                .Replace("{{subject}}", model.Subject)
                .Replace("{{fname}}", model.FirstName)
                .Replace("{{lname}}", model.LastName)
                .Replace("{{number}}", model.PhoneNumber)
                .Replace("{{from}}", model.From);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
          
            await SendEmail(msg);
        }

        public async void SendConfirmContactUsEmail(ContactUsAddRequest model)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            var from = new EmailAddress(_appKeys.AccountEmail);
            var subject = "Immersed: We recieved your email";
            var plainTextContent = "";
            var to = new EmailAddress(model.From);
            string path = Path.Combine(webRootPath, "EmailTemplates", "ContactUsConfirmation.html");
            var htmlContent = File.ReadAllText(path).Replace("{{message}}", model.Message)
                .Replace("{{booknow}}", $"{_appKeys.Domain}/signin")
                .Replace("{{Blogs}}", $"{_appKeys.Domain}/blogs")
                .Replace("{{aboutus}}", $"{_appKeys.Domain}/aboutpage")                
                .Replace("{{podcast}}", $"{_appKeys.Domain}/podcast");
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await SendEmail(msg);          

        }
        public async void SendConfirmEmail(string token, string email)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            var from = new EmailAddress("fakeEmail@dispostable.com", "Example User");
            var subject = "Immersed Registration Completion Next Step Needed";     
            string path = Path.Combine(webRootPath, "EmailTemplates", "EmailConfirmation.html");
            var htmlContent = File.ReadAllText(path).Replace("{{token}}", token).Replace("{{email}}", email);

            var message = new SendGridMessage()
            {
                From = from,
                Subject = subject,
                HtmlContent = htmlContent
            };
            message.AddTo(new EmailAddress(email)); 
            await SendEmail(message);
        }
 
        public async void SendInviteEmail(InviteMembersAddRequest model, string token, Organization organization)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            var from = new EmailAddress("immersed.contact@dispostable.com", "Immersed Team");
            var subject = $"{model.FirstName} {model.LastName}, you've been invited to join Immersed by {organization.Name}!";
            string path = Path.Combine(webRootPath, "EmailTemplates", "InviteMember.html");
            var htmlContent = File.ReadAllText(path).Replace("{{token}}", token)
                .Replace("{{email}}", model.Email)
                .Replace("{{firstName}}", model.FirstName)
                .Replace("{{lastName}}", model.LastName)
                .Replace("{{name}}", organization.Name)
                .Replace("{{organizationId}}", (model.OrganizationId).ToString());

            var message = new SendGridMessage()
            {
                From = from,
                Subject = subject,
                HtmlContent = htmlContent
            };
            message.AddTo(new EmailAddress(model.Email));
            await SendEmail(message);
        }

        public string GetTemplate()
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string path = "";
            path = Path.Combine(webRootPath, "EmailTemplates", "WelcomeTemplate.html");

            string template = File.ReadAllText(path);
            return template;
        }

        public async void PhishingEmail(string token, PhishingAddRequest model)
        {
            var fromEmail = new EmailAddress() 
            { 
                Email = model.FromEmail,
                Name = model.FromName
            };
        

            var toEmail = new EmailAddress()
            {
                Email = model.ToEmail,
                Name = model.ToName
            };

            var htmlContent = PhishingTemplate(token, model.ToEmail);
            var msg = MailHelper.CreateSingleEmail(fromEmail, toEmail, model.Subject, model.Body, htmlContent);
            await SendEmail(msg);
           
        }

        public string PhishingTemplate(string token, string email)
        {
            int tokenTypeId = (int)TokenType.TrainingEvent;
            string tokenType = tokenTypeId.ToString();
            string webRootPath = _webHostEnvironment.WebRootPath;
            string path = "";
            path = Path.Combine(webRootPath, "EmailTemplates", "PhishingEmail.html");
            string domain = _appKeys.Domain;
            string phishing = File.ReadAllText(path).Replace("{{domain}}", domain).Replace("{{token}}", token).Replace("{{email}}", email).Replace("{{tokenTypeId}}", tokenType);
            return phishing;
        }


    }
} 
