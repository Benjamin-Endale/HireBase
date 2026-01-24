using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HireBase_Backend.Data;
using HireBase_Backend.Models;
using HireBase_Backend.DTOs;
using HireBase_Backend.Services;

[ApiController]
[Route("api/[controller]")]
public class ApplicantJobController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly EmailService _emailService;

    public ApplicantJobController(AppDbContext db, EmailService emailService)
    {
        _db = db;
        _emailService = emailService;
    }

    // POST: api/applicant-job/save
    [HttpPost("save")]
    public async Task<IActionResult> SaveJob([FromBody] SaveJobRequest input)
    {
        // 1. Validate applicant
        var applicant = await _db.ApplicantRegistrations.FindAsync(input.ApplicantId);
        if (applicant == null)
            return NotFound(new { message = "Applicant not found." });

        // 2. Validate job
        var job = await _db.Jobs.FindAsync(input.JobId);
        if (job == null)
            return NotFound(new { message = "Job not found." });

        // 3. Check if already saved
        var exists = await _db.ApplicantJobs
            .AnyAsync(a => a.ApplicantId == input.ApplicantId && a.JobId == input.JobId);
        if (exists)
            return Conflict(new { message = "Job already saved." });

        // 4. Save job
        var applicantJob = new ApplicantJob
        {
            ApplicantId = input.ApplicantId,
            JobId = input.JobId
        };

        _db.ApplicantJobs.Add(applicantJob);
        await _db.SaveChangesAsync();

        try
        {
            var message = $"Job saved successfully for {applicant.Fullname}!\n\n" +
                          $"Saved Job Title: {job.JobTitle}\n\n" +
                          $"Keep going and complete your application process.\n\n" +
                          $"Thank you,\nHireBase Team";

            await _emailService.SendEmailAsync(applicant.Email!, "Job Saved", message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email: {ex.Message}");
        }

        return Ok(new { message = $"Job '{job.JobTitle}' saved successfully for {applicant.Fullname}." });
    }

    // GET: api/applicant-job/saved/{applicantId}
    [HttpGet("saved/{applicantId}")]
    public async Task<IActionResult> GetSavedJobs(Guid applicantId)
    {
        var jobs = await _db.ApplicantJobs
            .Where(a => a.ApplicantId == applicantId)
            .Include(a => a.Job)
            .Select(a => new
            {
                a.JobId,
                a.Job.JobTitle,
                a.Job.Location,
                a.Job.JobType,
                a.Job.CreatedAt,
                a.Job.ApplicationDeadline,
                a.Job.JobDescription,
                a.Job.Requirement,
                a.SavedAt,
                a.IsApplied
            })
            .ToListAsync();

        return Ok(jobs);
    }

    // GET: api/applicant-job/applied/{applicantId}
    [HttpGet("applied/{applicantId}")]
    public async Task<IActionResult> GetAppliedJobs(Guid applicantId)
    {
        // Check if applicant exists
        var applicantExists = await _db.ApplicantRegistrations.AnyAsync(a => a.Id == applicantId);
        if (!applicantExists)
            return NotFound(new { message = "Applicant not found." });

        // Get all jobs the applicant has applied to
        var jobs = await _db.ApplicantJobs
            .Where(a => a.ApplicantId == applicantId && a.IsApplied)
            .Include(a => a.Job)
            .Select(a => new
            {
                a.Job.JobTitle,
                a.Job.Location,
                a.Job.JobType,
                a.Job.CreatedAt,
                a.Job.ApplicationDeadline,
                a.Job.JobDescription,
                a.Job.Requirement,
                a.SavedAt
            })
            .ToListAsync();

        if (!jobs.Any())
            return NotFound(new { message = "No applied jobs found for this applicant." });

        return Ok(jobs);
    }
}
