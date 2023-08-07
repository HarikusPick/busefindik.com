using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace busefindik.com.MyHelpers
{
    public class EmailSender 
    {
        public static async Task SendEmail(string toEmail, string username, string subject, string message)
        {
            string apiKey = "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";
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress ("yunusfurkanozer@gmail.com", "busefindik.com");
            var to = new EmailAddress (toEmail, username);
            var plainTextContent = message;
            var htmlContent = "";

            var msg = MailHelper.CreateSingleEmail(
                from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);
        }
    }
}
