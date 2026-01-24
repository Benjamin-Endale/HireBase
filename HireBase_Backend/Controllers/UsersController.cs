using System;
using System.Linq;
using System.Threading.Tasks;
using HireBase_Backend.Data;
using HireBase_Backend.DTOs;
using HireBase_Backend.Models;
using HireBase_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HireBase_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher _hasher;
        private readonly EmailService _emailService;

        public UsersController(
            AppDbContext db,
            IPasswordHasher hasher,
            EmailService emailService)
        {
            _db = db;
            _hasher = hasher;
            _emailService = emailService;
        }

        // ============================
        // DTO
        // ============================
        public sealed class CreateUserDto
        {
            public string FullName { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? PhoneNumber { get; set; }
            public string Password { get; set; } = string.Empty;
            public string Role { get; set; } = "User";
            public Guid? TenantId { get; set; }
            public Guid? EmployeeId { get; set; }
        }

        // ============================
        // CREATE USER
        // ============================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto input)
        {
            if (string.IsNullOrWhiteSpace(input.Password))
                return BadRequest("Password is required.");

            var normalizedEmail = string.IsNullOrWhiteSpace(input.Email)
                ? null
                : input.Email.Trim().ToUpperInvariant();

            // Validate employee linkage
            if (input.EmployeeId != null)
            {
                var employee = await _db.Employees.FindAsync(input.EmployeeId);
                if (employee == null)
                    return BadRequest("Employee not found.");

                if (input.TenantId != employee.TenantId)
                    return BadRequest("TenantId of Employee must match with user.");
            }

            if (normalizedEmail != null)
            {
                var emailTaken = await _db.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail);
                if (emailTaken)
                    return Conflict("Email already in use.");
            }

            _hasher.Create(input.Password, out var hash, out var salt);

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = input.FullName.Trim(),
                Email = input.Email,
                NormalizedEmail = normalizedEmail,
                PhoneNumber = input.PhoneNumber,
                PasswordHash = hash,
                PasswordSalt = salt,
                TenantId = input.TenantId,
                EmployeeId = input.EmployeeId,
                SecurityStamp = Guid.NewGuid().ToString("N"),
                CreatedAt = DateTime.UtcNow,
                UserRoles = new List<UserRole>()
            };

            var roleId = await _db.Roles
                .Where(r => r.Name.ToUpper() == input.Role.ToUpper())
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            if (roleId == Guid.Empty)
                return BadRequest($"Role '{input.Role}' does not exist.");

            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = roleId
            });

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // Send welcome email
            try
            {
                var message =
                    $"Welcome {user.FullName},\n\n" +
                    $"Your HRMS account has been created.\n\n" +
                    $"Email: {user.Email}\n" +
                    $"Password: {input.Password}\n\n" +
                    $"Please keep this information secure.\n\n" +
                    $"HRMS Team";

                await _emailService.SendEmailAsync(
                    user.Email!,
                    "Welcome to HRMS",
                    message
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email failed: {ex.Message}");
            }

            return CreatedAtAction(nameof(GetById), new { id = user.Id },
                new { user.Id, user.FullName, user.Email, Role = input.Role });
        }

        // ============================
        // UPDATE PASSWORD
        // ============================
        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto input)
        {
            var user = await _db.Users.FindAsync(input.UserId);
            if (user == null)
                return NotFound("User not found.");

            if (!_hasher.Verify(input.OldPassword, user.PasswordHash, user.PasswordSalt))
                return BadRequest("Old password is incorrect.");

            _hasher.Create(input.NewPassword, out var hash, out var salt);

            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            user.SecurityStamp = Guid.NewGuid().ToString("N");
            user.UpdatedAt = DateTime.UtcNow;
            user.FullName = input.FullName;

            await _db.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                user.Email!,
                "Password Updated",
                $"Hello {user.FullName}, your password has been updated."
            );

            return Ok("Password updated successfully.");
        }

        // ============================
        // GET BY ID
        // ============================
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _db.Users
                .AsNoTracking()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.PhoneNumber,
                Roles = user.UserRoles.Select(ur => ur.Role!.Name).ToList(),
                user.TenantId,
                user.EmployeeId,
                user.IsActive,
                user.LastLoginUtc,
                user.CreatedAt,
                user.UpdatedAt
            });
        }

        // ============================
        // TOUCH LOGIN
        // ============================
        [HttpPost("{id:guid}/touch-login")]
        public async Task<IActionResult> TouchLogin(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.LastLoginUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(new { user.LastLoginUtc });
        }

        // ============================
        // DELETE USER
        // ============================
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // ============================
        // FORGOT PASSWORD (OTP)
        // ============================
        [HttpPost("forgot-password/request-otp")]
        public async Task<IActionResult> RequestPasswordOtp([FromBody] RequestPasswordOtpDto input)
        {
            var normalizedEmail = input.Email.Trim().ToUpperInvariant();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

            if (user == null)
                return NotFound("User not found.");

            var otp = new Random().Next(100000, 999999).ToString();
            user.PasswordResetOtp = otp;
            user.PasswordResetOtpExpires = DateTime.UtcNow.AddMinutes(5);

            await _db.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                user.Email!,
                "Password Reset OTP",
                $"Your OTP is: {otp}"
            );

            return Ok("OTP sent.");
        }

        [HttpPost("forgot-password/verify-otp")]
        public async Task<IActionResult> VerifyPasswordOtp([FromBody] VerifyOtpDto input)
        {
            var normalizedEmail = input.Email.Trim().ToUpperInvariant();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

            if (user == null)
                return NotFound("User not found.");

            if (user.PasswordResetOtp != input.Otp)
                return BadRequest("Invalid OTP.");

            if (user.PasswordResetOtpExpires < DateTime.UtcNow)
                return BadRequest("OTP expired.");

            return Ok("OTP verified.");
        }

        [HttpGet("by-tenant/{tenantId:guid}")]
        public async Task<IActionResult> GetUsersByTenantId(Guid tenantId)
        {
            var users = await _db.Users
                .AsNoTracking()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u => u.TenantId == tenantId)
                .Select(user => new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.PhoneNumber,
                    Roles = user.UserRoles.Select(ur => ur.Role!.Name).ToList(),
                    user.IsActive,
                    user.LastLoginUtc,
                    user.CreatedAt
                })
                .ToListAsync();


            return Ok(users);
        }


        [HttpPost("forgot-password/change")]
        public async Task<IActionResult> ChangePasswordWithOtp([FromBody] ChangePasswordWithOtpDto input)
        {
            var normalizedEmail = input.Email.Trim().ToUpperInvariant();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

            if (user == null)
                return NotFound("User not found.");

            if (user.PasswordResetOtp != input.Otp || user.PasswordResetOtpExpires < DateTime.UtcNow)
                return BadRequest("OTP invalid or expired.");

            _hasher.Create(input.NewPassword, out var hash, out var salt);

            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            user.PasswordResetOtp = null;
            user.PasswordResetOtpExpires = null;
            user.SecurityStamp = Guid.NewGuid().ToString("N");
            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                user.Email!,
                "Password Reset Successful",
                "Your password has been reset."
            );

            return Ok("Password reset successfully.");
        }
    }
}
