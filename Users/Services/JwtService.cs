using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using apitienda.Models;

namespace apitienda.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Genera un par de tokens: access token y refresh token.
        /// </summary>
        /// <param name="user">Usuario para quien se generan los tokens.</param>
        /// <returns>Tokens y datos relacionados.</returns>
        public (string AccessToken, string RefreshToken, string RefreshTokenJti, DateTimeOffset RefreshTokenExpiry, int AccessTokenExpiryMinutes) GenerateTokens(Usuario user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];
            var accessTokenExpiryMinutes = int.Parse(_configuration["Jwt:AccessTokenExpiryMinutes"] ?? "15");
            var refreshTokenExpiryDays = int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes);
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenExpiryDays);

            // Claims para access token
            var accessClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.email ?? ""),
                new Claim("role", user.role ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var accessToken = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: accessClaims,
                expires: accessTokenExpiry,
                signingCredentials: creds
            );

            string accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);

            // Claims para refresh token, con jti único
            var refreshJti = Guid.NewGuid().ToString();
            var refreshClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, refreshJti)
            };

            var refreshToken = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: refreshClaims,
                expires: refreshTokenExpiry,
                signingCredentials: creds
            );

            string refreshTokenString = new JwtSecurityTokenHandler().WriteToken(refreshToken);

            return (accessTokenString, refreshTokenString, refreshJti, refreshTokenExpiry, accessTokenExpiryMinutes);
        }
    }
}
