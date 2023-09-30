using System.Net.Mail;
using System.Net;
using TreinoSportAPI.Services.Interfaces;

namespace TreinoSportAPI.Services {
    public class EmailService : IEmailService {

        public Task SendPasswordCode(string email, string token) {

            var emailTreinoSport = "softwarecolossus@outlook.com";

            var smtpClient = new SmtpClient() {
                Host = "smtp-mail.outlook.com",
                Port = 587,
                Credentials = new NetworkCredential(emailTreinoSport, "Funestus3,14$"),
                EnableSsl = true,
            };

            var corpoEmail = File.ReadAllText(Environment.CurrentDirectory + "\\Utilities\\templateEmail.html");
            corpoEmail = corpoEmail.Replace("@", token);

            smtpClient.UseDefaultCredentials = false;
            var mailMessage = new MailMessage {
                From = new MailAddress(emailTreinoSport),
                Subject = "Redefinição de senha - TreinoSport",
                Body = corpoEmail,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            return smtpClient.SendMailAsync(mailMessage);
        }

    }
}
