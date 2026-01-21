namespace HireBase_Backend.Models
{
    public class Interview
    {
        public Guid Id { get; set; }

        public Guid? ShortlistId { get; set; }
        public Shortlist? Shortlist { get; set; } = null!;

        public DateTime? ScheduledTime { get; set; }

        public int? Duration { get; set; }

        public DateTime? ScheduledDate { get; set; }

        public string? MeetingUrl { get; set; }

        public string InterviewNote { get; set; } = string.Empty;

        public string? Status { get; set; }

    }
}
