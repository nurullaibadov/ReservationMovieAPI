using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MovieReservationAPI.Infrastructure.Settings;
using ReservationMovieAPI.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReservationAPI.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailSettings> settings,
            ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendReservationConfirmationAsync(
            string toEmail, string userName, string reservationCode,
            string movieTitle, DateTime screeningTime, string hallName,
            List<string> seatLabels, decimal totalAmount)
        {
            var seatsText = string.Join(", ", seatLabels);
            var subject = $"Rezervasyon Onayı - {reservationCode}";

            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; background: #f4f4f4; }}
        .container {{ max-width: 600px; margin: 30px auto; 
                      background: white; border-radius: 10px; 
                      padding: 30px; }}
        .header {{ background: #e50914; color: white; 
                   padding: 20px; border-radius: 8px 8px 0 0;
                   text-align: center; }}
        .info-box {{ background: #f9f9f9; border-left: 4px solid #e50914;
                     padding: 15px; margin: 15px 0; }}
        .footer {{ color: #888; font-size: 12px; text-align: center;
                   margin-top: 20px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎬 Rezervasyon Onaylandı!</h1>
        </div>
        <p>Merhaba <strong>{userName}</strong>,</p>
        <p>Rezervasyonunuz başarıyla oluşturulmuştur.</p>
        
        <div class='info-box'>
            <p>📋 <strong>Rezervasyon Kodu:</strong> {reservationCode}</p>
            <p>🎬 <strong>Film:</strong> {movieTitle}</p>
            <p>📅 <strong>Tarih/Saat:</strong> 
               {screeningTime:dd MMMM yyyy HH:mm}</p>
            <p>🏢 <strong>Salon:</strong> {hallName}</p>
            <p>💺 <strong>Koltuklar:</strong> {seatsText}</p>
            <p>💰 <strong>Toplam:</strong> {totalAmount:C2}</p>
        </div>
        
        <p>Lütfen rezervasyon kodunuzu gösterime gelirken yanınızda bulundurunuz.</p>
        
        <div class='footer'>
            <p>© {DateTime.Now.Year} Movie Reservation System</p>
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(toEmail, subject, htmlBody);
        }

        public async Task SendReservationCancellationAsync(
            string toEmail, string userName, string reservationCode)
        {
            var subject = $"Rezervasyon İptali - {reservationCode}";
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial; background: #f4f4f4; padding: 20px;'>
    <div style='max-width:600px; margin:auto; background:white; 
                border-radius:10px; padding:30px;'>
        <h2 style='color:#e50914;'>Rezervasyon İptal Edildi</h2>
        <p>Merhaba <strong>{userName}</strong>,</p>
        <p>
            <strong>{reservationCode}</strong> kodlu rezervasyonunuz 
            başarıyla iptal edilmiştir.
        </p>
        <p>İade işlemi 3-5 iş günü içinde gerçekleşecektir.</p>
    </div>
</body>
</html>";

            await SendEmailAsync(toEmail, subject, htmlBody);
        }

        public async Task SendPasswordResetEmailAsync(
            string toEmail, string userName, string resetLink)
        {
            var subject = "Şifre Sıfırlama Talebi";
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial; background: #f4f4f4; padding: 20px;'>
    <div style='max-width:600px; margin:auto; background:white;
                border-radius:10px; padding:30px;'>
        <h2 style='color:#e50914;'>Şifre Sıfırlama</h2>
        <p>Merhaba <strong>{userName}</strong>,</p>
        <p>Şifre sıfırlama talebiniz alındı.</p>
        <p>
            <a href='{resetLink}' 
               style='background:#e50914; color:white; padding:12px 24px;
                      border-radius:5px; text-decoration:none;'>
               Şifremi Sıfırla
            </a>
        </p>
        <p style='color:#888; font-size:12px;'>
            Bu link 1 saat geçerlidir. Talebi siz yapmadıysanız 
            dikkate almayınız.
        </p>
    </div>
</body>
</html>";

            await SendEmailAsync(toEmail, subject, htmlBody);
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var subject = "Movie Reservation System'e Hoş Geldiniz!";
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial; background: #f4f4f4; padding: 20px;'>
    <div style='max-width:600px; margin:auto; background:white;
                border-radius:10px; padding:30px; text-align:center;'>
        <h1 style='color:#e50914;'>🎬 Hoş Geldiniz!</h1>
        <p>Merhaba <strong>{userName}</strong>,</p>
        <p>Movie Reservation System ailesine katıldığınız için teşekkürler!</p>
        <p>Artık en yeni filmleri kolayca rezerve edebilirsiniz.</p>
    </div>
</body>
</html>";

            await SendEmailAsync(toEmail, subject, htmlBody);
        }

        // ─── Ortak Email Gönderme ────────────────────────────────────────────────
        private async Task SendEmailAsync(
            string toEmail, string subject, string htmlBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _settings.FromName, _settings.FromEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                await client.ConnectAsync(
                    _settings.Host,
                    _settings.Port,
                    SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(
                    _settings.Username, _settings.Password);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation(
                    "Email sent to {Email}: {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                // Email gönderimi başarısız olursa sistemi durdurma — sadece logla
                _logger.LogError(ex,
                    "Failed to send email to {Email}", toEmail);
            }
        }
    }
}
