using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HireBase_Backend.Models
{
    [Table("applicant_jobs")]
    public class ApplicantJob
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ApplicantId { get; set; }

        [ForeignKey("ApplicantId")]
        public ApplicantRegistration Applicant { get; set; } = null!;

        [Required]
        public Guid JobId { get; set; }

        [ForeignKey("JobId")]
        public Job Job { get; set; } = null!;

        public DateTime SavedAt { get; set; } = DateTime.UtcNow;

        // Optional: mark if applied
        public bool IsApplied { get; set; } = false;
    }
}
