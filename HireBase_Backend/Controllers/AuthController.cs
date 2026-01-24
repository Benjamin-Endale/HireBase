using System;
using System.Linq;
using System.Threading.Tasks;
using HireBase_Backend.Data;
using HireBase_Backend.Models;
using HireBase_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtTokenService _jwt;
        private readonly EmailService _email;

        public AuthController(AppDbContext db, IPasswordHasher hasher, IJwtTokenService jwt, EmailService email)
        {
            _db = db;
            _hasher = hasher;
            _jwt = jwt;
            _email = email;
        }

        // Request/Response DTOs
        public record LoginRequest(string UsernameOrEmail, string Password);
        public record OtpVerifyRequest(string UsernameOrEmail, string OtpCode);
        public record ResendOtpRequest(string UsernameOrEmail);
        public record EmailLoginRequest(string Email);

        public record LoginResponse(
            string? id = null,
            string? accessToken = null,
            DateTimeOffset? expiresAt = null,
            string? refreshToken = null,
            DateTimeOffset? refreshExpiresAt = null,
            string? role = null,
            string? FullName = null,
            string? email = null,
            string? message = null,
            Guid? tenantId = null,
            bool requiresOtp = false,
            bool otpVerified = false
        );

        // POST: /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest input)
        {
            if (string.IsNullOrWhiteSpace(input.UsernameOrEmail))
                return BadRequest("Email is required.");

            var emailNorm = input.UsernameOrEmail.Trim().ToUpperInvariant();

            var user = await _db.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.NormalizedEmail == emailNorm);

            if (user == null || !user.IsActive)
                return Unauthorized("Invalid email or inactive user.");

            if (!_hasher.Verify(input.Password, user.PasswordHash, user.PasswordSalt))
            {
                user.AccessFailedCount++;
                await _db.SaveChangesAsync();
                return Unauthorized("Invalid credentials.");
            }

            // Generate OTP
            var otp = new Random().Next(100000, 999999).ToString();
            user.OtpCode = otp;
            user.OtpExpiryUtc = DateTime.UtcNow.AddMinutes(5);
            await _db.SaveChangesAsync();

            if (string.IsNullOrWhiteSpace(user.Email))
                return BadRequest("User does not have a valid email.");

            await _email.SendOtpAsync(user.Email, otp);

            return Ok(new LoginResponse(
                id: user.Id.ToString(),
                email: user.Email,
                message: "OTP sent to your email.",
                requiresOtp: true,
                otpVerified: false
            ));
        }

        // POST: /api/auth/verify-otp
        [HttpPost("verify-otp")]
        public async Task<ActionResult<LoginResponse>> VerifyOtp([FromBody] OtpVerifyRequest input)
        {
            var norm = input.UsernameOrEmail.Trim().ToUpperInvariant();
            var user = await _db.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.NormalizedEmail == norm);

            if (user == null || !user.IsActive)
                return Unauthorized("Invalid credentials.");

            if (user.OtpCode == null || user.OtpExpiryUtc < DateTime.UtcNow)
                return Unauthorized("OTP expired.");

            if (user.OtpCode != input.OtpCode)
                return Unauthorized("Invalid OTP.");

            // Reset OTP & update last login
            user.OtpCode = null;
            user.OtpExpiryUtc = null;
            user.LastLoginUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var (jwt, exp, _) = await _jwt.CreateAccessTokenAsync(user);
            var (rt, rtExp) = _jwt.CreateRefreshToken();

            var roles = user.UserRoles.Select(ur => ur.Role!.Name).ToList();

            return Ok(new LoginResponse(
                id: user.Id.ToString(),
                accessToken: jwt,
                expiresAt: exp,
                refreshToken: rt,
                refreshExpiresAt: rtExp,
                role: string.Join(",", roles),
                FullName: user.FullName,
                email: user.Email,
                tenantId: user.TenantId,
                requiresOtp: false,
                otpVerified: true
            ));
        }

        // POST: /api/auth/resend-otp
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest input)
        {
            if (string.IsNullOrWhiteSpace(input.UsernameOrEmail))
                return BadRequest("Username or email is required.");

            var norm = input.UsernameOrEmail.Trim().ToUpperInvariant();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == norm);

            if (user == null || !user.IsActive)
                return Unauthorized("User not found or inactive.");

            var otp = new Random().Next(100000, 999999).ToString();
            user.OtpCode = otp;
            user.OtpExpiryUtc = DateTime.UtcNow.AddMinutes(5);
            await _db.SaveChangesAsync();

            if (string.IsNullOrWhiteSpace(user.Email))
                return BadRequest("User does not have a valid email.");

            await _email.SendOtpAsync(user.Email, otp);

            return Ok(new
            {
                id = user.Id.ToString(),
                message = "New OTP sent to your email.",
                email = user.Email
            });
        }

        // POST: /api/auth/email-login
        [HttpPost("email-login")]
        public async Task<IActionResult> LoginByEmail([FromBody] EmailLoginRequest input)
        {
            if (string.IsNullOrWhiteSpace(input.Email))
                return BadRequest("Email is required.");

            var emailNorm = input.Email.Trim().ToUpperInvariant();
            var user = await _db.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.NormalizedEmail == emailNorm);

            if (user == null || !user.IsActive)
                return Unauthorized("Invalid email or inactive user.");

            // Update last login
            user.LastLoginUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var (jwt, exp, _) = await _jwt.CreateAccessTokenAsync(user);
            var (rt, rtExp) = _jwt.CreateRefreshToken();
            var roles = user.UserRoles.Select(ur => ur.Role!.Name).ToList();

            return Ok(new LoginResponse(
                id: user.Id.ToString(),
                accessToken: jwt,
                expiresAt: exp,
                refreshToken: rt,
                refreshExpiresAt: rtExp,
                role: string.Join(",", roles),
                FullName: user.FullName,
                email: user.Email,
                message: "Login successful via email.",
                requiresOtp: false,
                otpVerified: true
            ));
        }
    }
}
