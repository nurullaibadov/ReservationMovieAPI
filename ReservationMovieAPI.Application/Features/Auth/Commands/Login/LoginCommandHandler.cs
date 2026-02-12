using MediatR;
using Microsoft.AspNetCore.Identity;
using ReservationMovieAPI.Application.Common.Models;
using ReservationMovieAPI.Application.DTOs.Auth;
using ReservationMovieAPI.Application.Interfaces.Services;
using ReservationMovieAPI.Domain.Entities;
using ReservationMovieAPI.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationMovieAPI.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler
        : IRequestHandler<LoginCommand, ApiResponse<TokenDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<ApiResponse<TokenDto>> Handle(
            LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException(
                    "Email veya şifre hatalı.");

            // Hesap kilitli mi?
            if (await _userManager.IsLockedOutAsync(user))
                throw new BusinessRuleException(
                    "Hesabınız geçici olarak kilitlenmiştir. " +
                    "15 dakika sonra tekrar deneyin.");

            var result = await _signInManager.CheckPasswordSignInAsync(
                user, request.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    throw new BusinessRuleException(
                        "Çok fazla hatalı giriş. Hesabınız kilitlendi.");

                throw new UnauthorizedAccessException(
                    "Email veya şifre hatalı.");
            }

            // Son giriş zamanını güncelle
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var tokenDto = await _tokenService.GenerateTokensAsync(user);

            // Refresh token kaydet
            user.RefreshToken = tokenDto.RefreshToken;
            user.RefreshTokenExpiryTime = tokenDto.RefreshTokenExpiry;
            await _userManager.UpdateAsync(user);

            return ApiResponse<TokenDto>.SuccessResult(
                tokenDto, "Giriş başarılı.");
        }
    }
}
