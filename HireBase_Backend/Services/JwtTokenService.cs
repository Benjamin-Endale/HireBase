using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HireBase_Backend.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HireBase_Backend.Services
{
    // Make sure your interface IJwtTokenService has these two methods:
    // Task<(string Jwt, DateTimeOffset ExpiresAt, string Jti)> CreateAccessTokenAsync(ApplicantRegistration applicant);
    // (string RefreshToken, DateTimeOffset ExpiresAt) CreateRefreshToken();

    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        // =====================================
        // User ACCESS TOKEN
        // =====================================

        public Task<(string Jwt, DateTimeOffset ExpiresAt, string Jti)> CreateAccessTokenAsync(User user)
        {
            // Read settings
            var key = _config["JwtSettings:Key"] ?? throw new InvalidOperationException("JwtSettings:Key missing");
            var iss = _config["JwtSettings:Issuer"];
            var aud = _config["JwtSettings:Audience"];
            var mins = int.TryParse(_config["JwtSettings:AccessTokenMinutes"], out var m) ? m : 60;

            var jti = Guid.NewGuid().ToString("N");
            var now = DateTimeOffset.UtcNow;
            var exp = now.AddMinutes(mins);

            // Get all role names assigned to the user
            var roleNames = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),

                // Helpful identity claims
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("security_stamp", user.SecurityStamp),

                // Optional context claims
                new Claim("tenant_id", user.TenantId?.ToString() ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };


            // Convert the array to a list first
            var claimsList = claims.ToList();

            // Add roles
            foreach (var roleName in roleNames)
            {
                claimsList.Add(new Claim(ClaimTypes.Role, roleName));
            }

            var permissions = user.UserRoles?
                .SelectMany(ur => System.Text.Json.JsonSerializer.Deserialize<List<string>>(ur.Role.PermissionsJson) ?? new List<string>())
                .Distinct()
                .ToList() ?? new List<string>();

            foreach (var perm in permissions)
            {
                claimsList.Add(new Claim("permission", perm));
            }



            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: iss,
                audience: aud,
                claims: claimsList,
                notBefore: now.UtcDateTime,
                expires: exp.UtcDateTime,
                signingCredentials: credentials
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Task.FromResult((jwt, exp, jti));
        }



        // =====================================
        // APPLICANT ACCESS TOKEN
        // =====================================
        public Task<(string Jwt, DateTimeOffset ExpiresAt, string Jti)>
    CreateAccessTokenForApplicantAsync(ApplicantRegistration applicant)
        {
            var key = _config["JwtSettings:Key"]
                      ?? throw new InvalidOperationException("JwtSettings:Key missing");

            var issuer = _config["JwtSettings:Issuer"];
            var audience = _config["JwtSettings:Audience"];
            var minutes = int.TryParse(_config["JwtSettings:AccessTokenMinutes"], out var m) ? m : 60;

            var now = DateTimeOffset.UtcNow;
            var expires = now.AddMinutes(minutes);
            var jti = Guid.NewGuid().ToString("N");

            var claims = new List<Claim>
            {
                // Subject = applicant identity
                new Claim(JwtRegisteredClaimNames.Sub, applicant.Email),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),

                // Custom applicant claims
                new Claim("applicant_id", applicant.Id.ToString()),
                new Claim("applicant_email", applicant.Email),
                new Claim("applicant_fullname", applicant.Fullname ?? string.Empty)
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: now.UtcDateTime,
                expires: expires.UtcDateTime,
                signingCredentials: credentials
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Task.FromResult((jwt, expires, jti));
        }

        // =====================================
        // REFRESH TOKEN
        // =====================================
        public (string RefreshToken, DateTimeOffset ExpiresAt)
            CreateRefreshToken()
        {
            var minutes = int.TryParse(_config["JwtSettings:RefreshTokenMinutes"], out var m) ? m : 7 * 24 * 60;
            var expires = DateTimeOffset.UtcNow.AddMinutes(minutes);

            var bytes = RandomNumberGenerator.GetBytes(32);
            var token = Convert.ToBase64String(bytes);

            return (token, expires);
        }
    }
}
