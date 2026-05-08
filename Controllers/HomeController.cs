using KiefProductions_Site.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Web;

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
            ViewData["Title"] = "Kief Productions | Event Production Company";
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
                using var client = new System.Net.Mail.SmtpClient(_smtpSettings.Host, _smtpSettings.Port);
                client.EnableSsl = _smtpSettings.UseSsl;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = 10000;

                message.From = new MailAddress(_smtpSettings.Username, "Kief Productions Website");
                message.ReplyToList.Add(new MailAddress(model.Email, model.Name));
                message.To.Add(new MailAddress(_smtpSettings.RecipientEmail, _smtpSettings.RecipientName));
                message.Subject = $"New Enquiry from {model.Name}";
                message.Body = BuildEmailBody(model);
                message.IsBodyHtml = true;

                client.Send(message);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Contact form error");
                return Json(new { success = false, message = "Something went wrong. Please try again." });
            }
        }

        private static string BuildEmailBody(ContactForm model)
        {
            string eventDate = model.Date != default ? model.Date.ToString("dd MMMM yyyy") : "Not specified";
            string phone = string.IsNullOrWhiteSpace(model.PhoneNumber) ? "Not provided" : model.PhoneNumber;

            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head><meta charset=""UTF-8"" /><meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" /></head>
<body style=""margin:0;padding:0;background-color:#0d0d0d;font-family:'Helvetica Neue',Helvetica,Arial,sans-serif;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#0d0d0d;padding:40px 0;"">
    <tr>
      <td align=""center"">
        <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""max-width:600px;width:100%;"">

          <!-- HEADER -->
          <tr>
            <td style=""background-color:#111111;border-top:4px solid #ff3e00;border-radius:12px 12px 0 0;padding:36px 40px;text-align:center;"">
              <p style=""margin:0 0 6px;font-size:11px;letter-spacing:4px;text-transform:uppercase;color:#ff3e00;font-weight:600;"">Event Production</p>
              <h1 style=""margin:0;font-size:28px;font-weight:700;color:#ffffff;letter-spacing:2px;text-transform:uppercase;"">Kief Productions</h1>
              <p style=""margin:12px 0 0;font-size:13px;color:#888888;"">New enquiry received via website</p>
            </td>
          </tr>

          <!-- BODY -->
          <tr>
            <td style=""background-color:#1a1a1a;padding:36px 40px;"">

              <p style=""margin:0 0 24px;font-size:16px;color:#cccccc;line-height:1.6;"">
                You have a new contact form submission. Details are below — hit reply to respond directly to <strong style=""color:#ffffff;"">{System.Web.HttpUtility.HtmlEncode(model.Name)}</strong>.
              </p>

              <!-- DETAILS CARD -->
              <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#242424;border-radius:8px;border:1px solid #2e2e2e;margin-bottom:24px;"">
                <tr>
                  <td style=""padding:20px 24px;border-bottom:1px solid #2e2e2e;"">
                    <p style=""margin:0 0 4px;font-size:11px;letter-spacing:2px;text-transform:uppercase;color:#ff3e00;"">Name</p>
                    <p style=""margin:0;font-size:16px;color:#ffffff;font-weight:600;"">{System.Web.HttpUtility.HtmlEncode(model.Name)}</p>
                  </td>
                </tr>
                <tr>
                  <td style=""padding:20px 24px;border-bottom:1px solid #2e2e2e;"">
                    <p style=""margin:0 0 4px;font-size:11px;letter-spacing:2px;text-transform:uppercase;color:#ff3e00;"">Email</p>
                    <p style=""margin:0;font-size:16px;color:#ffffff;""><a href=""mailto:{System.Web.HttpUtility.HtmlEncode(model.Email)}"" style=""color:#00a8ff;text-decoration:none;"">{System.Web.HttpUtility.HtmlEncode(model.Email)}</a></p>
                  </td>
                </tr>
                <tr>
                  <td style=""padding:20px 24px;border-bottom:1px solid #2e2e2e;"">
                    <p style=""margin:0 0 4px;font-size:11px;letter-spacing:2px;text-transform:uppercase;color:#ff3e00;"">Phone</p>
                    <p style=""margin:0;font-size:16px;color:#ffffff;"">{System.Web.HttpUtility.HtmlEncode(phone)}</p>
                  </td>
                </tr>
                <tr>
                  <td style=""padding:20px 24px;border-bottom:1px solid #2e2e2e;"">
                    <p style=""margin:0 0 4px;font-size:11px;letter-spacing:2px;text-transform:uppercase;color:#ff3e00;"">Event Date</p>
                    <p style=""margin:0;font-size:16px;color:#ffffff;"">{eventDate}</p>
                  </td>
                </tr>
                <tr>
                  <td style=""padding:20px 24px;"">
                    <p style=""margin:0 0 8px;font-size:11px;letter-spacing:2px;text-transform:uppercase;color:#ff3e00;"">Message</p>
                    <p style=""margin:0;font-size:15px;color:#cccccc;line-height:1.7;white-space:pre-wrap;"">{System.Web.HttpUtility.HtmlEncode(model.Details)}</p>
                  </td>
                </tr>
              </table>

              <!-- REPLY BUTTON -->
              <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
                <tr>
                  <td align=""center"">
                    <a href=""mailto:{System.Web.HttpUtility.HtmlEncode(model.Email)}"" style=""display:inline-block;background-color:#ff3e00;color:#ffffff;text-decoration:none;font-size:15px;font-weight:600;padding:14px 40px;border-radius:50px;letter-spacing:1px;"">
                      Reply to {System.Web.HttpUtility.HtmlEncode(model.Name)}
                    </a>
                  </td>
                </tr>
              </table>

            </td>
          </tr>

          <!-- FOOTER -->
          <tr>
            <td style=""background-color:#111111;border-radius:0 0 12px 12px;padding:24px 40px;text-align:center;border-top:1px solid #2e2e2e;"">
              <p style=""margin:0 0 4px;font-size:12px;color:#555555;"">Kief Productions &mdash; Pretoria, Gauteng, South Africa</p>
              <p style=""margin:0;font-size:12px;color:#555555;"">info@kiefproductions.co.za</p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
        }

        public IActionResult Privacy()
        {
            ViewData["HideNavbar"] = null;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
