using HireBase_Backend.Data;
using HireBase_Backend.DTOs;
using HireBase_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HireBase_Backend.Filters;

namespace HRMS.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RoleAuthorize("SuperAdmin , SystemAdmin")]
    public class InterviewsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InterviewsController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Schedule (Create) Interview
        [HttpPost]
        public async Task<IActionResult> CreateInterview([FromBody] InterviewCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ApplicantEmail) ||
                string.IsNullOrWhiteSpace(dto.JobTitle))
            {
                return BadRequest(new { message = "ApplicantEmail and JobTitle are required." });
            }

            // Validate date
            if (dto.ScheduledDate.Date < DateTime.UtcNow.Date)
            {
                return BadRequest(new { message = "ScheduledDate cannot be in the past." });
            }

            var shortlist = await _context.Shortlists
                .Include(s => s.Job)
                .FirstOrDefaultAsync(s =>
                    s.Email == dto.ApplicantEmail &&
                    s.Job.JobTitle == dto.JobTitle);

            if (shortlist == null)
                return NotFound(new { message = "No shortlist found for the given Email and Job Title." });

            var existingInterview = await _context.Interviews
                .FirstOrDefaultAsync(i => i.ShortlistId == shortlist.ShortlistID);

            if (existingInterview != null)
                return BadRequest(new { message = "Cannot schedule the same applicant twice." });

            var interview = new Interview
            {
                Id = Guid.NewGuid(),
                ShortlistId = shortlist.ShortlistID,
                ScheduledDate = dto.ScheduledDate,
                Duration = dto.Duration,
                LocationUrl = dto.LocationUrl,
                MeetingUrl = dto.MeetingUrl,
                InterviewNote = dto.InterviewNote,
                Mode = dto.Mode,
                Status = "Scheduled",
                ScheduledOn = DateTime.UtcNow
            };

            _context.Interviews.Add(interview);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Interview created successfully.",
                data = interview
            });
        }

        // 2. Edit Interview
        [HttpPut("edit")]
        public async Task<IActionResult> EditInterview([FromBody] InterviewCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ApplicantEmail) ||
                string.IsNullOrWhiteSpace(dto.JobTitle))
            {
                return BadRequest(new { message = "ApplicantEmail and JobTitle are required." });
            }

            var shortlist = await _context.Shortlists
                .Include(s => s.Job)
                .FirstOrDefaultAsync(s =>
                    s.Email == dto.ApplicantEmail &&
                    s.Job.JobTitle == dto.JobTitle);

            if (shortlist == null)
                return NotFound(new { message = "Shortlist not found for the given Email and Job Title." });

            var interview = await _context.Interviews
                .FirstOrDefaultAsync(i => i.ShortlistId == shortlist.ShortlistID);

            if (interview == null)
                return NotFound(new { message = "Interview not found." });

            if (dto.ScheduledDate.Date < DateTime.UtcNow.Date)
            {
                return BadRequest(new { message = "ScheduledDate cannot be in the past." });
            }

            interview.ScheduledDate = dto.ScheduledDate;
            interview.Duration = dto.Duration;
            interview.LocationUrl = dto.LocationUrl;
            interview.MeetingUrl = dto.MeetingUrl;
            interview.InterviewNote = dto.InterviewNote;
            interview.Mode = dto.Mode;
            interview.ScheduledOn = DateTime.UtcNow;

            _context.Interviews.Update(interview);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Interview updated successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInterviews()
        {
            var interviews = await _context.Interviews
                .Include(i => i.Shortlist)
                    .ThenInclude(s => s.Job)
 
                .Select(i => new InterviewDTO
                {
                    Id = i.Id,
                    ShortlistId = i.ShortlistId,

                    // Applicant info comes from Shortlist
                    ApplicantName = i.Shortlist.Name,
                    ApplicantEmail = i.Shortlist.Email ?? string.Empty,
                    Position = i.Shortlist.position ?? string.Empty,
                    JobTitle = i.Shortlist.Job.JobTitle,

 
                    // Interview details
                    Type = i.Mode,
                    Status = i.Status,
                    ScheduledDate = i.ScheduledDate,
                    Duration = i.Duration,

                    LocationUrl = i.LocationUrl,
                    MeetingUrl = i.MeetingUrl,
                    InterviewNote = i.InterviewNote
                })
                .ToListAsync();

            return Ok(new
            {
                message = "All interviews fetched successfully.",
                data = interviews
            });
        }

        // 3. Delete Interview
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInterview(Guid id)
        {
            var interview = await _context.Interviews.FindAsync(id);

            if (interview == null)
                return NotFound(new { message = "Interview not found." });

            _context.Interviews.Remove(interview);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Interview deleted successfully." });
        }
    }
}
