namespace HireBase_Backend.DTOs
{
    public class InterviewDTO
    {
        public Guid Id { get; set; }
        public Guid ShortlistId { get; set; }
        public string ApplicantName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;

        public Guid? InterviewerId { get; set; }
        public string InterviewerName { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;   // Online/Offline
        public string Status { get; set; } = "Scheduled";

        public DateTime? ScheduledDate { get; set; }
        public string? LocationUrl { get; set; }
        public string? MeetingUrl { get; set; }
        public int? Duration { get; set; }
        public string InterviewNote { get; set; } = string.Empty;

        public string ApplicantEmail { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;

        public string InterviewerEmail { get; set; } = string.Empty;

    }

    public class InterviewCreateDto
    {
        public string ApplicantEmail { get; set; }
        public string JobTitle { get; set; }

        public DateTime ScheduledDate { get; set; }   //  DateTime, NOT string
        public int Duration { get; set; }

        public string? LocationUrl { get; set; }
        public string? MeetingUrl { get; set; }
        public string? InterviewNote { get; set; }

        public string Mode { get; set; } // Online / Onsite
    }

}
