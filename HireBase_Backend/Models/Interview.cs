namespace Jobposting.HireBase_Backend.Models
{
    public class Interview
    {
        public Guid Id { get; set; }

        public Guid? JobId { get; set; }
        public Job? Job { get; set; } = null!;

        public Guid? ApplicantId { get; set; }       // Changed from int? to Guid?
        public Applicant? Applicant { get; set; } = null!;

        public DateTime? ScheduledTime { get; set; }

        public int? Duration { get; set; }

        public DateTime? ScheduledDate { get; set; }

        public string? MeetingUrl { get; set; }

        public string InterviewNote { get; set; } = string.Empty;

        public string? Status { get; set; }

    }
}
