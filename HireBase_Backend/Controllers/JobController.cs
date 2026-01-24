using HireBase_Backend.Data;
using HireBase_Backend.Models;
using HireBase_Backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HireBase_Backend.Filters;

namespace HireBase_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JobController(AppDbContext context)
        {
            _context = context;
        }

        // CREATE Job
        [HttpPost]
        [RoleAuthorize("SuperAdmin , SystemAdmin ")]
        public async Task<IActionResult> CreateJob([FromBody] JobCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var job = new Job
            {
                Id = Guid.NewGuid(),
                JobTitle = dto.JobTitle,
                TenantID = dto.TenantID,
                Location = dto.Location,
                JobType = dto.JobType,
                SalaryRange = dto.SalaryRange,
                ApplicationDeadline = dto.ApplicationDeadline,
                JobDescription = dto.JobDescription,
                Requirement = dto.Requirement,
                CreatedAt = DateTime.UtcNow
            };

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            var tenant = await _context.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == job.TenantID);

            var response = new
            {
                job.Id,
                job.JobTitle,
                job.JobType,
                job.SalaryRange,
                job.Location,
                job.ApplicationDeadline,
                job.JobDescription,
                job.Requirement,
                TenantName = tenant?.Name ?? "N/A",
                job.CreatedAt
            };

            var today = DateTime.UtcNow.Date;
            var activeJobsCount = await _context.Jobs
                .CountAsync(j => j.ApplicationDeadline.HasValue && j.ApplicationDeadline >= today);

            return Ok(new
            {
                message = "Job created successfully.",
                data = response,
                ActiveJobsCount = activeJobsCount
            });
        }

        // READ All Jobs
        [HttpGet("data")]
        public async Task<IActionResult> GetJob()
        {
            var today = DateTime.UtcNow.Date;

            var jobs = await _context.Jobs
                .Include(j => j.Tenant)
                .AsNoTracking()
                .Select(j => new
                {
                    id = j.Id,
                    jobTitle = j.JobTitle,
                    jobType = j.JobType,
                    salaryRange = j.SalaryRange,
                    location = j.Location,
                    applicationDeadline = j.ApplicationDeadline,
                    jobDescription = j.JobDescription,
                    requirement = j.Requirement,
                    TenantName = j.Tenant != null ? j.Tenant.Name : "N/A",
                    createdAt = j.CreatedAt
                })
                .ToListAsync();

            var activeJobsCount = await _context.Jobs
                .CountAsync(j => j.ApplicationDeadline.HasValue && j.ApplicationDeadline >= today);

            if (jobs.Count == 0)
                return NotFound(new { message = "No jobs available." });

            return Ok(new
            {
                message = "Jobs retrieved successfully.",
                data = jobs,
                activeJobsCount
            });
        }

        // READ Job by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobById(Guid id)
        {
            var job = await _context.Jobs
                .Include(j => j.Tenant)
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null) return NotFound();

            var response = new
            {
                job.Id,
                job.JobTitle,
                job.JobType,
                job.SalaryRange,
                job.Location,
                job.ApplicationDeadline,
                job.JobDescription,
                job.Requirement,
                TenantName = job.Tenant != null ? job.Tenant.Name : "N/A",
                job.CreatedAt,
                job.UpdatedAt
            };

            return Ok(response);
        }

        // UPDATE Job
        [HttpPut("{id}")]

        [RoleAuthorize("SuperAdmin , SystemAdmin ")]
        public async Task<IActionResult> UpdateJob(Guid id, [FromBody] JobCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return NotFound(new { message = $"Job with Id {id} not found." });

            job.JobTitle = dto.JobTitle;
            job.TenantID = dto.TenantID;
            job.Location = dto.Location;
            job.JobType = dto.JobType;
            job.SalaryRange = dto.SalaryRange;
            job.ApplicationDeadline = dto.ApplicationDeadline;
            job.JobDescription = dto.JobDescription;
            job.Requirement = dto.Requirement;
            job.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var tenant = await _context.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == job.TenantID);

            var response = new
            {
                job.Id,
                job.JobTitle,
                job.JobType,
                job.SalaryRange,
                job.Location,
                job.ApplicationDeadline,
                job.JobDescription,
                job.Requirement,
                TenantName = tenant?.Name ?? "N/A",
                job.UpdatedAt
            };

            var today = DateTime.UtcNow.Date;
            var activeJobsCount = await _context.Jobs
                .CountAsync(j => j.ApplicationDeadline.HasValue && j.ApplicationDeadline >= today);

            return Ok(new
            {
                message = "Job updated successfully.",
                data = response,
                ActiveJobsCount = activeJobsCount
            });
        }

        // DELETE Job
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(Guid id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound();

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Job deleted successfully" });
        }

        // GET: api/job/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveJobs()
        {
            var today = DateTime.UtcNow.Date;

            var activeJobs = await _context.Jobs
                .Where(j => j.ApplicationDeadline.HasValue && j.ApplicationDeadline >= today)
                .OrderBy(j => j.ApplicationDeadline)
                .Select(j => new
                {
                    j.JobTitle,
                    j.Location,
                    j.ApplicationDeadline
                })
                .ToListAsync();

            if (activeJobs.Count == 0)
                return NotFound(new { message = "No active jobs available." });

            return Ok(activeJobs);
        }

        // GET: api/job/active/count
        [HttpGet("active/count")]
        public async Task<IActionResult> GetActiveJobsCount()
        {
            var today = DateTime.UtcNow.Date;

            var activeJobsCount = await _context.Jobs
                .CountAsync(j => j.ApplicationDeadline.HasValue && j.ApplicationDeadline >= today);

            return Ok(new { activeJobs = activeJobsCount });
        }

        // GET: api/job/search
        [HttpGet("search")]
        public async Task<IActionResult> SearchJobs([FromQuery] string? jobType, [FromQuery] double? hoursAgo)
        {
            var query = _context.Jobs
                .Include(j => j.Tenant)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(jobType) || (hoursAgo.HasValue && hoursAgo > 0))
            {
                var since = DateTime.UtcNow.AddHours(-(hoursAgo ?? 0));

                query = query.Where(j =>
                    (!string.IsNullOrWhiteSpace(jobType) && j.JobType.Contains(jobType)) ||
                    (hoursAgo.HasValue && j.CreatedAt >= since)
                );
            }

            var jobs = await query
                .Select(j => new
                {
                    j.JobTitle,
                    j.JobType,
                    j.Location,
                    j.CreatedAt,
                    HoursAgo = Math.Round((DateTime.UtcNow - j.CreatedAt).TotalHours, 1),
                    j.ApplicationDeadline,
                    j.SalaryRange,
                    j.JobDescription,
                    j.Requirement,
                    TenantName = j.Tenant != null ? j.Tenant.Name : "N/A"
                })
                .ToListAsync();

            if (jobs.Count == 0)
                return NotFound(new { message = "No jobs match the search criteria." });

            return Ok(jobs);
        }

        // GET: api/job/dashboard/tenant/{tenantId}
        [HttpGet("dashboard/tenant/{tenantId}")]
        public async Task<IActionResult> GetJobsByTenantOnly(Guid tenantId)
        {
            var today = DateTime.UtcNow.Date;

            var jobs = await _context.Jobs
                .Include(j => j.Tenant)
                .AsNoTracking()
                .Where(j => j.TenantID == tenantId
                            && j.ApplicationDeadline.HasValue
                            && j.ApplicationDeadline >= today)
                .Select(j => new
                {
                    j.Id,
                    j.JobTitle,
                    j.JobType,
                    j.SalaryRange,
                    j.Location,
                    j.ApplicationDeadline,
                    j.JobDescription,
                    j.Requirement,
                    TenantName = j.Tenant != null ? j.Tenant.Name : "N/A",
                    j.CreatedAt
                })
                .ToListAsync();

            var activeJobsCount = jobs.Count;
            var jobIds = jobs.Select(j => j.Id).ToList();
            var totalApplications = await _context.ApplicantJobs
                .CountAsync(a => jobIds.Contains(a.JobId));
            var interviewsToday = await _context.Interviews
                .CountAsync(i => i.ScheduledOn.HasValue && i.ScheduledOn.Value.Date == today);

            return Ok(new
            {
                message = "Tenant-level dashboard retrieved successfully.",
                activeJobsCount,
                totalApplications,
                interviewsToday,
                jobs
            });
        }
    }
}
