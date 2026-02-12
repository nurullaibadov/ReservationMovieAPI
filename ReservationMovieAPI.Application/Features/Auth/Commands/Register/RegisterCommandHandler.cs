using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ReservationMovieAPI.Application.Common.Models;
using ReservationMovieAPI.Domain.Entities;
using ReservationMovieAPI.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler
       : IRequestHandler<RegisterCommand, ApiResponse<TokenDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            ILogger<RegisterCommandHandler> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<ApiResponse<TokenDto>> Handle(
            RegisterCommand request, CancellationToken cancellationToken)
        {
            // Email daha önce kayıtlı mı?
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                throw new BusinessRuleException(
                    "Bu email adresi zaten kullanımda.");

            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse<TokenDto>.FailureResult(
                    "Kullanıcı oluşturulamadı.", errors);
            }

            // Default rol: Customer
            await _userManager.AddToRoleAsync(user, "Customer");

            // Token üret
            var tokenDto = await _tokenService.GenerateTokensAsync(user);

            // Refresh token'ı kaydet
            user.RefreshToken = tokenDto.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("New user registered: {Email}", user.Email);

            return ApiResponse<TokenDto>.SuccessResult(
                tokenDto, "Kayıt başarılı. Hoş geldiniz!");
        }
    }
}
