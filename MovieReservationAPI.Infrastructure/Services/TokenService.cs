using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MovieReservationAPI.Infrastructure.Settings;
using ReservationMovieAPI.Application.DTOs.Auth;
using ReservationMovieAPI.Application.Interfaces.Services;
using ReservationMovieAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MovieReservationAPI.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(
            IOptions<JwtSettings> jwtSettings,
            UserManager<ApplicationUser> userManager)
        {
            _jwtSettings = jwtSettings.Value;
            _userManager = userManager;
        }

        public async Task<TokenDto> GenerateTokensAsync(ApplicationUser user)
        {
            // Kullanıcının rollerini getir
            var roles = await _userManager.GetRolesAsync(user);

            // Claims — token içine gömülecek bilgiler
            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email!)
        };

            // Her rolü ayrı claim olarak ekle
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // İmzalama anahtarı
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var accessTokenExpiry = DateTime.UtcNow
                .AddMinutes(_jwtSettings.ExpirationInMinutes);

            // JWT token oluştur
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = accessTokenExpiry,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            // Refresh token — kriptografik güvenli rastgele string
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow
                .AddDays(_jwtSettings.RefreshTokenExpirationInDays);

            return new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = accessTokenExpiry,
                RefreshTokenExpiry = refreshTokenExpiry,
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Roles = roles.ToList()
            };
        }

        public async Task<TokenDto> RefreshTokenAsync(
            string accessToken, string refreshToken)
        {
            // Süresi dolmuş token'dan principal'ı çıkar
            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
                throw new UnauthorizedAccessException("Geçersiz access token.");

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Token'da kullanıcı bilgisi yok.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");

            // Refresh token eşleşiyor mu?
            if (user.RefreshToken != refreshToken)
                throw new UnauthorizedAccessException("Geçersiz refresh token.");

            // Refresh token süresi dolmuş mu?
            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token süresi dolmuş.");

            // Yeni token çifti oluştur
            var newTokens = await GenerateTokensAsync(user);

            // Yeni refresh token'ı kaydet
            user.RefreshToken = newTokens.RefreshToken;
            user.RefreshTokenExpiryTime = newTokens.RefreshTokenExpiry;
            await _userManager.UpdateAsync(user);

            return newTokens;
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // Süresi dolmuş olsa da doğrula
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_jwtSettings.Secret))
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(
                    token, tokenValidationParameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(
                        SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private static string GenerateRefreshToken()
        {
            // 64 byte = 512 bit entropi — brute force imkânsız
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
