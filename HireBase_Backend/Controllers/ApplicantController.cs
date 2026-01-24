using HireBase_Backend.Data;
using HireBase_Backend.DTOs;
using HireBase_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HireBase_Backend.Filters;

namespace HireBase_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicantController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ApplicantController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ==================================
        // CREATE - Add New Applicant
        // ==================================
        [HttpPost]
        public async Task<IActionResult> CreateApplicant([FromForm] ApplicantDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Console.WriteLine($"Received Name: {dto.Name}, Email: {dto.Email}, ResumeFile: {dto.ResumeUrl?.FileName}");

            // Check if applicant already applied
            if (dto.JobId.HasValue && !string.IsNullOrWhiteSpace(dto.Email))
            {
                var emailLower = dto.Email.Trim().ToLower();
                var alreadyApplied = await _context.Applicants
                    .AnyAsync(a => a.Email != null &&
                                   a.Email.ToLower() == emailLower &&
                                   a.JobId == dto.JobId.Value);

                if (alreadyApplied)
                    return BadRequest(new { message = "You have already applied for this job." });
            }

            string? jobTitle = null;

            if (dto.JobId.HasValue)
            {
                var job = await _context.Jobs
                    .FirstOrDefaultAsync(j => j.Id == dto.JobId.Value);

                if (job == null)
                    return BadRequest(new { message = "Invalid Job ID" });

                jobTitle = job.JobTitle;
            }

            string? resumeUrl = null;
            if (dto.ResumeUrl != null)
            {
                var rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadsFolder = Path.Combine(rootPath, "uploads", "applicants", "resumes");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(dto.ResumeUrl.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ResumeUrl.CopyToAsync(stream);
                }

                resumeUrl = $"/uploads/applicants/resumes/{fileName}";
            }
            else
            {
                Console.WriteLine("No file received!");
            }

            var applicant = new Applicant
            {
                Id = Guid.NewGuid(),
                JobId = dto.JobId,
                ApplicantRegistrationId = dto.ApplicantRegistrationId,
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                ResumeUrl = resumeUrl ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                position = jobTitle
            };

            _context.Applicants.Add(applicant);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                displayJson = new
                {
                    applicant.Id,
                    applicant.JobId,
                    applicant.Name,
                    applicant.Email,
                    applicant.Phone,
                    applicant.position,
                    ResumeUrl = applicant.ResumeUrl,
                    ApplicationsSubmittedDate = applicant.CreatedAt
                }
            });
        }

        // ==================================
        // READ - Get All Applicants
        // ==================================
        [HttpGet]
        [RoleAuthorize("SuperAdmin ,SystemAdmin")]
        public async Task<IActionResult> GetAllApplicants()
        {
            var applicants = await _context.Applicants
                .Include(a => a.Job)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new
                {
                    a.Id,
                    a.Name,
                    a.Email,
                    a.Phone,
                    a.ResumeUrl,
                    a.Status,
                    a.position,
                    JobTitle = a.Job != null ? a.Job.JobTitle : "N/A"
                })
                .ToListAsync();

            return Ok(applicants);
        }

        // ==================================
        // READ - Get Applicant by ID
        // ==================================
        [HttpGet("{id:guid}")]
        [RoleAuthorize("SuperAdmin ,SystemAdmin")]
        public async Task<IActionResult> GetApplicantById(Guid id)
        {
            var applicant = await _context.Applicants
                .Include(a => a.Job)
                .Where(a => a.Id == id)
                .Select(a => new
                {
                    a.Id,
                    a.Name,
                    a.Email,
                    a.Phone,
                    a.ResumeUrl,
                    a.Status,
                    a.position,
                    JobTitle = a.Job != null ? a.Job.JobTitle : "N/A"
                })
                .FirstOrDefaultAsync();

            if (applicant == null)
                return NotFound(new { message = "Applicant not found" });

            return Ok(applicant);
        }

        // ==================================
        // DELETE - Remove Applicant
        // ==================================
        [HttpDelete("{id:guid}")]
        [RoleAuthorize("SuperAdmin ,SystemAdmin")]
        public async Task<IActionResult> DeleteApplicant(Guid id)
        {
            var applicant = await _context.Applicants.FindAsync(id);
            if (applicant == null)
                return NotFound(new { message = "Applicant not found" });

            _context.Applicants.Remove(applicant);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Applicant deleted successfully" });
        }

        // Display simplified list
        [HttpGet("display")]
        public async Task<IActionResult> GetApplicantsDisplay()
        {
            var applicants = await _context.Applicants
                .Select(a => new
                {
                    a.Name,
                    a.Email,
                    a.Phone,
                    a.position,
                    ApplicationsSubmittedDate = a.CreatedAt
                })
                .ToListAsync();

            return Ok(applicants);
        }

        // Total Applicants
        [HttpGet("total")]
        [RoleAuthorize("SuperAdmin ,SystemAdmin")]
        public async Task<IActionResult> GetTotalApplicants()
        {
            var total = await _context.Applicants.CountAsync();
            return Ok(new { TotalApplicants = total });
        }


        [HttpGet("applicants-with-summary/tenant/{tenantId}")]
        public async Task<IActionResult> GetApplicantsWithSummaryByTenant(Guid tenantId)
        {
            var today = DateTime.Today;

            // TOTAL APPLICATIONS
            int totalApplications = await _context.Applicants
                .Where(a =>
                    a.Job != null &&
                    a.Job.TenantID == tenantId)
                .CountAsync();

            // ACTIVE JOBS
            int activeJobs = await _context.Jobs
                .Where(j =>
                    j.TenantID == tenantId &&
                    j.ApplicationDeadline >= today)
                .CountAsync();

            // INTERVIEWS TODAY (FIXED)
            int interviewsToday = await _context.Interviews
                .Where(i =>
                    i.ScheduledDate != null &&
                    i.ScheduledDate.Value.Date == today &&
                    i.Shortlist != null &&
                    i.Shortlist.Job != null &&
                    i.Shortlist.Job.TenantID == tenantId)
                .CountAsync();

            // APPLICANTS
            var applicants = await _context.Applicants
                .Include(a => a.Job)
                .Where(a =>
                    a.Job != null &&
                    a.Job.TenantID == tenantId)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new
                {
                    a.Id,
                    a.Name,
                    a.Email,
                    a.Phone,
                    a.Status,
                    a.position,
                    JobTitle = a.Job!.JobTitle,
                    ApplicationsSubmittedDate = a.CreatedAt
                })
                .ToListAsync();

            // FINAL RESPONSE
            return Ok(new
            {
                summary = new
                {
                    activeJobs,
                    totalApplications,
                    interviewsToday
                },
                applicants = applicants
            });
        }


        // Search Applicants
        [HttpGet("search")]
        [RoleAuthorize("SuperAdmin ,SystemAdmin")]
        public async Task<IActionResult> GetApplicantsByNameAndPosition(
            [FromQuery] string? name,
            [FromQuery] string? position)
        {
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(position))
                return BadRequest(new { message = "Please provide either a name or a position to search." });

            var query = _context.Applicants.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(a => a.Name.ToLower().Contains(name.Trim().ToLower()));

            if (!string.IsNullOrWhiteSpace(position))
                query = query.Where(a => !string.IsNullOrEmpty(a.position) && a.position.ToLower().Contains(position.Trim().ToLower()));

            var applicants = await query
                .Select(a => new
                {
                    a.Id,
                    a.Name,
                    a.Email,
                    a.Phone,
                    Position = a.position,
                    ApplicationsSubmittedDate = a.CreatedAt
                })
                .ToListAsync();

            if (!applicants.Any())
                return NotFound(new { message = "No applicants found for the given criteria." });

            return Ok(applicants);
        }

        [HttpGet("applied/{applicantId}")]
        public async Task<IActionResult> GetAppliedJobs(Guid applicantId)
        {
            // Check if applicant exists
            var applicantExists = await _context.ApplicantRegistrations.AnyAsync(a => a.Id == applicantId);
            if (!applicantExists)
                return NotFound(new { message = "Applicant not found." });

            // Get all jobs the applicant has applied to
            var jobs = await _context.Applicants
                .Where(a => a.ApplicantRegistrationId == applicantId)
                .Include(a => a.Job)
 
                .Include(a => a.Job)
 
                        .ThenInclude(d => d.Tenant)
                .Select(a => new
                {
                    JobId = a.Job!.Id,
                    a.Job.JobTitle,
                    a.Job.Location,
                    a.Job.JobType,
                    a.Job.CreatedAt,
                    a.Job.ApplicationDeadline,
                    a.Job.JobDescription,
                    a.Job.Requirement,

 
                })
                .ToListAsync();


            return Ok(jobs);
        }


        // Get Resume by Applicant ID
        [HttpGet("{applicantId:guid}/resume")]
        public async Task<IActionResult> GetResumeByApplicantId(Guid applicantId)
        {
            var applicant = await _context.Applicants.FindAsync(applicantId);

            if (applicant == null)
                return NotFound(new { message = "Applicant not found." });

            if (string.IsNullOrWhiteSpace(applicant.ResumeUrl))
                return NotFound(new { message = "Resume not uploaded." });

            var rootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var filePath = Path.Combine(rootPath, applicant.ResumeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "Resume file not found on server." });

            var fileExtension = Path.GetExtension(filePath).ToLower();
            string contentType = fileExtension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                _ => "application/octet-stream"
            };

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, contentType, Path.GetFileName(filePath));
        }
    }
}
