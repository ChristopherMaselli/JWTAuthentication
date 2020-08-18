using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using JWTAuthentication.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace JWTAuthentication.Controllers
{
    [Route("api/Mail")]
    [ApiController]
    public class MailController : Controller
    {
        [HttpPost("Send")]
        public IActionResult SendMail(EmailData emailData)
        {
            
            // create email message
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emailData.from);
            email.To.Add(MailboxAddress.Parse(emailData.to));
            email.Subject = emailData.subject;
            email.Body = new TextPart(TextFormat.Plain) { Text = emailData.body };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("Theoriginalchris1@gmail.com", "Password");
            smtp.Send(email);
            smtp.Disconnect(true);

            IActionResult response = Ok(new { message = "Email Sent!" });
            return response;
        }

        public class EmailData
        {
            public string from { get; set; }

            public string to { get; set; }

            public string subject { get; set; }

            public string body { get; set; }
        }
    }
}