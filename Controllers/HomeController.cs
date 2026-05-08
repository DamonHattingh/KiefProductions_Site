using KiefProductions_Site.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace KiefProductions_Site.Controllers
{
    public class HomeController : Controller
    {

        // Holds SMTP settings injected from appsettings.json
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IOptions<SmtpSettings> smtpOptions) 
        {
            _logger = logger;
            _smtpSettings = smtpOptions.Value;
        }

        public IActionResult Index()
        {
            ViewData["HideNavbar"] = true;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(ContactForm model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Please fill in all required fields." });
            }
            try
            {
                using var message = new MailMessage();
                using var client = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("info.kiefklank@gmail.com", "zbbwowycrnyecjoj");
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = 10000;

                message.From = new MailAddress("info@kiefproductions.co.za", "Kief Website");
                message.ReplyToList.Add(new MailAddress(model.Email, model.Name));
                message.To.Add(new MailAddress("info@kiefproductions.co.za", "Kief Productions"));
                message.Subject = "Contact Form: " + model.Name;
                message.Body = $"Name: {model.Name}\n" +
                              $"Email: {model.Email}\n" +
                              $"Phone: {model.PhoneNumber}\n" +
                              $"Date: {model.Date.ToShortDateString()}\n\n" +
                              $"Message:\n{model.Details}\n\n---\nSubmitted by: {model.Name} <{model.Email}>";

                client.Send(message);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Contact form error");
                return Json(new { success = false, message = "Something went wrong. Please try again." });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
