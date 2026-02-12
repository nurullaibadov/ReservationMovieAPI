using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendReservationConfirmationAsync(
            string toEmail, string userName,
            string reservationCode, string movieTitle,
            DateTime screeningTime, string hallName,
            List<string> seatLabels, decimal totalAmount);

        Task SendReservationCancellationAsync(
            string toEmail, string userName,
            string reservationCode);

        Task SendPasswordResetEmailAsync(
            string toEmail, string userName, string resetLink);

        Task SendWelcomeEmailAsync(
            string toEmail, string userName);
    }
}
