using System;
using System.Threading.Tasks;
using HireBase_Backend.Data;
using HireBase_Backend.Models;
using HireBase_Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HireBase_Backend.Controllers
{
    [ApiController]
    [Route("api/applicants")]
    public class ApplicantRegistrationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly EmailService _emailService;

        public ApplicantRegistrationController(AppDbContext context, IPasswordHasher passwordHasher, EmailService emailService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }

        // POST: api/applicants/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterApplicantDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _context.ApplicantRegistrations
                .FirstOrDefaultAsync(a => a.Email == dto.Email);
            if (existing != null)
                return Conflict("Email already registered.");

            _passwordHasher.Create(dto.Password, out var hash, out var salt);

            var applicant = new ApplicantRegistration
            {
                Fullname = dto.Fullname,
                Email = dto.Email,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            _context.ApplicantRegistrations.Add(applicant);
            await _context.SaveChangesAsync();

            try
            {
                var message = $"Welcome {dto.Fullname},\n\n" +
                              $"Your Applicant account has been created.\n\n" +
                              $"Login details:\n" +
                              $"Email: {dto.Email}\n" +
                              $"Password: {dto.Password}\n\n" +
                              $"Please keep this information secure.\n\n" +
                              $"Best regards,\nHRMS Team";

                await _emailService.SendEmailAsync(dto.Email!, "Welcome to HRMS", message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                // Optional: log or handle gracefully, but don’t fail registration
            }

            return Ok("Applicant registered successfully.");
        }

        // POST: api/applicants/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginApplicantDto dto)
        {
            var applicant = await _context.ApplicantRegistrations
                .FirstOrDefaultAsync(a => a.Email == dto.Email);

            if (applicant == null)
                return Unauthorized("Invalid email or password.");

            var verified = _passwordHasher.Verify(dto.Password, applicant.PasswordHash, applicant.PasswordSalt);
            if (!verified)
                return Unauthorized("Invalid email or password.");

            return Ok("Login successful.");
        }
    }

    // DTOs
    public class RegisterApplicantDto
    {
        [Required]
        public string Fullname { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;


    }

    public class LoginApplicantDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
