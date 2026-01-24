using System;
using System.ComponentModel.DataAnnotations;

namespace HireBase_Backend.DTOs
{
    public class ApplicantDTO
    {
        // FK -> Jobs(Id)
        public Guid? JobId { get; set; }


        [Required]
        public Guid ApplicantRegistrationId { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [EmailAddress, MaxLength(255)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        public IFormFile? ResumeUrl { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        [MaxLength(100)]
        public string? position { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
