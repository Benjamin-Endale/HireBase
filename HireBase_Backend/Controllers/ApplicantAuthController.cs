using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HireBase_Backend.Data;
using HireBase_Backend.Models;
using HireBase_Backend.Services;
using HireBase_Backend.DTOs;

namespace HireBase_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicantAuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtTokenService _jwt;
        private readonly EmailService _email;

        public ApplicantAuthController(AppDbContext db, IPasswordHasher hasher, IJwtTokenService jwt, EmailService email)
        {
            _db = db;
            _hasher = hasher;
            _jwt = jwt;
            _email = email;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] ApplicantRegisterRequest input)
        {
            if (await _db.ApplicantRegistrations.AnyAsync(a => a.Email.ToUpper() == input.Email.Trim().ToUpper()))
                return Conflict("Email already exists.");

            _hasher.Create(input.Password, out var hash, out var salt);

            var applicant = new ApplicantRegistration
            {
                Id = Guid.NewGuid(),
                Fullname = input.Fullname,
                Email = input.Email,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            _db.ApplicantRegistrations.Add(applicant);
            await _db.SaveChangesAsync();

            return Created("", new
            {
                applicant.Id,
                applicant.Fullname,
                applicant.Email
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ApplicantLoginRequest input)
        {
            var emailNorm = input.Email.Trim().ToUpper();
            var applicant = await _db.ApplicantRegistrations
                .FirstOrDefaultAsync(a => a.Email.ToUpper() == emailNorm);

            if (applicant == null || !_hasher.Verify(input.Password, applicant.PasswordHash, applicant.PasswordSalt))
                return Unauthorized("Invalid email or password.");

            // Generate OTP
            var otp = new Random().Next(100000, 999999).ToString();
            applicant.OtpCode = otp;
            applicant.OtpExpiryUtc = DateTime.UtcNow.AddMinutes(5);
            await _db.SaveChangesAsync();

            await _email.SendOtpAsync(applicant.Email, otp);

            return Ok(new ApplicantLoginResponse
            {
                Id = applicant.Id.ToString(),
                Email = applicant.Email,
                Fullname = applicant.Fullname,
                Message = "OTP sent to your email.",
                RequiresOtp = true
            });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyRequest input)
        {
            var emailNorm = input.UsernameOrEmail.Trim().ToUpper();
            var applicant = await _db.ApplicantRegistrations
                .FirstOrDefaultAsync(a => a.Email.ToUpper() == emailNorm);

            if (applicant == null)
                return Unauthorized("Invalid email.");

            if (applicant.OtpCode != input.OtpCode || applicant.OtpExpiryUtc < DateTime.UtcNow)
                return Unauthorized("OTP invalid or expired.");

            // Clear OTP
            applicant.OtpCode = null;
            applicant.OtpExpiryUtc = null;
            await _db.SaveChangesAsync();

            // Generate JWT and refresh token
            var jwtResult = await _jwt.CreateAccessTokenForApplicantAsync(applicant);
            var jwt = jwtResult.Jwt;

            var refreshTokenResult = _jwt.CreateRefreshToken();

            return Ok(new ApplicantLoginResponse
            {
                Id = applicant.Id.ToString(),
                AccessToken = jwt,
                RefreshToken = refreshTokenResult.RefreshToken,
                RefreshExpiresAt = refreshTokenResult.ExpiresAt,
                Fullname = applicant.Fullname,
                Email = applicant.Email,
                Message = "Login successful.",
                RequiresOtp = false,
                OtpVerified = true
            });
        }

        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdateApplicantPasswordDto input)
        {
            var applicant = await _db.ApplicantRegistrations.FindAsync(input.applicantId);
            if (applicant == null)
                return NotFound("Applicant not found.");

            if (!_hasher.Verify(input.OldPassword, applicant.PasswordHash, applicant.PasswordSalt))
                return BadRequest("Old password is incorrect.");

            _hasher.Create(input.NewPassword, out var newHash, out var newSalt);
            applicant.PasswordHash = newHash;
            applicant.PasswordSalt = newSalt;
            applicant.Fullname = input.FullName;

            await _db.SaveChangesAsync();

            try
            {
                await _email.SendOtpAsync(applicant.Email, "Your password has been updated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email failed: {ex.Message}");
            }

            return Ok("Password updated successfully.");
        }

        [HttpPut("update-name")]
        public async Task<IActionResult> UpdateName([FromBody] UpdateApplicantNameDto input)
        {
            var applicant = await _db.ApplicantRegistrations.FindAsync(input.applicantId);
            if (applicant == null)
                return NotFound("Applicant not found.");

            applicant.Fullname = input.FullName;
            await _db.SaveChangesAsync();

            try
            {
                await _email.SendOtpAsync(applicant.Email, "Your name has been updated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email failed: {ex.Message}");
            }

            return Ok("Name updated successfully.");
        }

        [HttpPost("forgot-password/request-otp")]
        public async Task<IActionResult> RequestPasswordOtp([FromBody] RequestPasswordOtpDto input)
        {
            var emailNorm = input.Email.Trim().ToUpper();
            var applicant = await _db.ApplicantRegistrations
                .FirstOrDefaultAsync(a => a.Email.ToUpper() == emailNorm);

            if (applicant == null)
                return NotFound("Applicant not found.");

            var otp = new Random().Next(100000, 999999).ToString();
            applicant.OtpCode = otp;
            applicant.OtpExpiryUtc = DateTime.UtcNow.AddMinutes(5);
            await _db.SaveChangesAsync();

            await _email.SendOtpAsync(applicant.Email, otp);

            return Ok("OTP sent to your email.");
        }

        [HttpPost("forgot-password/verify-otp")]
        public async Task<IActionResult> VerifyPasswordOtp([FromBody] VerifyOtpDto input)
        {
            var emailNorm = input.Email.Trim().ToUpper();
            var applicant = await _db.ApplicantRegistrations
                .FirstOrDefaultAsync(a => a.Email.ToUpper() == emailNorm);

            if (applicant == null)
                return NotFound("Applicant not found.");

            if (applicant.OtpCode != input.Otp)
                return BadRequest("Invalid OTP.");

            if (applicant.OtpExpiryUtc < DateTime.UtcNow)
                return BadRequest("OTP expired.");

            return Ok("OTP verified successfully.");
        }

        [HttpPost("forgot-password/change")]
        public async Task<IActionResult> ChangePasswordWithOtp([FromBody] ChangePasswordWithOtpDto input)
        {
            var emailNorm = input.Email.Trim().ToUpper();
            var applicant = await _db.ApplicantRegistrations
                .FirstOrDefaultAsync(a => a.Email.ToUpper() == emailNorm);

            if (applicant == null)
                return NotFound("Applicant not found.");

            if (applicant.OtpCode != input.Otp || applicant.OtpExpiryUtc < DateTime.UtcNow)
                return BadRequest("OTP invalid or expired.");

            _hasher.Create(input.NewPassword, out var hash, out var salt);
            applicant.PasswordHash = hash;
            applicant.PasswordSalt = salt;
            applicant.OtpCode = null;
            applicant.OtpExpiryUtc = null;

            await _db.SaveChangesAsync();

            await _email.SendOtpAsync(applicant.Email, "Your password has been reset successfully.");

            return Ok("Password reset successfully.");
        }
    }
}
