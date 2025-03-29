using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;


public class PasswordResetEmail
    {
        private readonly string _smtpServer = "smtp.gmail.com"; // O el servidor de tu elección
        private readonly int _smtpPort = 587; // Puerto SMTP
        private readonly string _smtpUser = "tu-correo@gmail.com"; // Tu correo electrónico
        private readonly string _smtpPassword = "tu-contraseña"; // Tu contraseña de correo o App Password

        public async Task SendPasswordResetEmail(string email, string token)
        {
            var resetLink = $"https://tusitio.com/reset-password?token={token}"; // Enlace de restablecimiento con token

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser),
                Subject = "Restablecimiento de Contraseña",
                Body = $"<p>Haz clic en el siguiente enlace para restablecer tu contraseña:</p><a href=\"{resetLink}\">Restablecer contraseña</a>",
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                smtpClient.EnableSsl = true;
                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }