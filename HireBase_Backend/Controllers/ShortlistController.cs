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
    [RoleAuthorize("SuperAdmin ,SystemAdmin")]
    public class ShortlistController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ShortlistController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET: api/shortlist
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAllShortlists()
        {
            var shortlists = await _context.Shortlists
                .Select(s => new
                {
                    s.ShortlistID,
                    s.position,
                    s.Name,
                    s.Status,
                    s.ShortlistedOn
                })
                .ToListAsync();

            return Ok(shortlists);
        }

        // =========================
        // POST: api/shortlist/move/{applicantId}
        // =========================
        [HttpPost("move/{applicantId}")]
        public async Task<IActionResult> MoveApplicantToShortlist(Guid applicantId)
        {
            var applicant = await _context.Applicants
                .FirstOrDefaultAsync(a => a.Id == applicantId);

            if (applicant == null)
                return NotFound(new { message = "Applicant not found." });

            if (!applicant.JobId.HasValue)
                return BadRequest(new { message = "Applicant must have a JobID." });

            var alreadyShortlisted = await _context.Shortlists
                .AnyAsync(s =>
                    s.JobID == applicant.JobId.Value &&
                    !string.IsNullOrEmpty(s.Email) &&
                    s.Email == applicant.Email);

            if (alreadyShortlisted)
                return BadRequest(new { message = "Applicant is already shortlisted for this job." });

            var shortlist = new Shortlist
            {
                JobID = applicant.JobId.Value,
                Name = applicant.Name,
                Email = applicant.Email,
                Phone = applicant.Phone,
                ResumeUrl = applicant.ResumeUrl,
                position = applicant.position,
                Status = "Shortlist",
                ShortlistedOn = DateTime.UtcNow
            };

            _context.Shortlists.Add(shortlist);
            _context.Applicants.Remove(applicant);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                shortlist.ShortlistID,
                shortlist.position,
                shortlist.Name,
                shortlist.Status,
                shortlist.ShortlistedOn
            });
        }

        // =========================
        // GET: api/shortlist/job/{jobId}
        // =========================
        [HttpGet("job/{jobId}")]
        public async Task<IActionResult> GetShortlistsByJob(Guid jobId)
        {
            var shortlists = await _context.Shortlists
                .Where(s => s.JobID == jobId)
                .Select(s => new ShortlistDTO
                {
                    ShortlistID = s.ShortlistID,
                    JobID = s.JobID,
                    Name = s.Name,
                    Email = s.Email,
                    Phone = s.Phone,
                    ResumeUrl = s.ResumeUrl,
                    position = s.position,
                    Status = s.Status,
                    ShortlistedOn = s.ShortlistedOn
                })
                .ToListAsync();

            if (!shortlists.Any())
                return NotFound(new { message = "No shortlists found for this job." });

            return Ok(shortlists);
        }

        // =========================
        // DELETE: api/shortlist/{id}
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShortlist(Guid id)
        {
            var shortlist = await _context.Shortlists.FindAsync(id);

            if (shortlist == null)
                return NotFound(new { message = "Shortlist record not found." });

            _context.Shortlists.Remove(shortlist);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Shortlist record deleted successfully." });
        }

        // =========================
        // GET: api/shortlist/tenant/{tenantId}
        // =========================
        [HttpGet("tenant/{tenantId}")]
        public async Task<IActionResult> GetShortlistedWithSummaryByTenant(Guid tenantId)
        {
            var today = DateTime.UtcNow.Date;

            var shortlisted = await _context.Shortlists
                .Include(s => s.Job)
                .Where(s => s.Job != null && s.Job.TenantID == tenantId)
                .Select(s => new
                {
                    s.ShortlistID,
                    s.JobID,
                    s.Name,
                    s.Email,
                    s.Phone,
                    s.ResumeUrl,
                    jobTitle = s.position,
                    s.Status,
                    s.ShortlistedOn
                })
                .ToListAsync();

            var activeJobs = await _context.Jobs
                .Where(j =>
                    j.TenantID == tenantId &&
                    j.ApplicationDeadline != null &&
                    j.ApplicationDeadline >= today)
                .CountAsync();

            var totalApplications = await _context.Shortlists
                .Include(s => s.Job)
                .Where(s => s.Job != null && s.Job.TenantID == tenantId)
                .CountAsync();

            var interviewsToday = await _context.Interviews
                .Include(i => i.Shortlist)
                .ThenInclude(s => s.Job)
                .Where(i =>
                    i.ScheduledDate != null &&
                    i.ScheduledDate.Value.Date == today &&
                    i.Shortlist != null &&
                    i.Shortlist.Job != null &&
                    i.Shortlist.Job.TenantID == tenantId)
                .CountAsync();

            return Ok(new
            {
                summary = new
                {
                    activeJobs,
                    totalApplications,
                    interviewsToday
                },
                shortlistedApplicants = shortlisted
            });
        }
    }
}
