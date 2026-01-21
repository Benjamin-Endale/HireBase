using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HireBase_Backend.Models
{
    public class Shortlist
    {
        public Guid Id { get; set; }  // Changed from int to GUID

        // FK -> Jobs(Id)
        [Required(ErrorMessage = "Job ID is required")]
        public Guid? JobId { get; set; }       // Changed from int? to Guid?
        public Job? Job { get; set; } = null!;


        // FK -> Recruiter(Id)
        [Required(ErrorMessage = "Recruiter ID is required")]
        public Guid? RecruiterId { get; set; }       // Changed from int? to Guid?
        public Recruiter? Recruiter { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Column("normalized_email"), MaxLength(255)]
        public string? NormalizedEmail { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(50)]
        [Column("status")]
        public string? Status { get; set; }

        [Required, MaxLength(200)]
        public string Github { get; set; } = string.Empty;

        [EmailAddress, MaxLength(255)]
        public string? Portfolio { get; set; }

        [MaxLength(50)]
        public string? Linkedin { get; set; }

        [MaxLength(50)]
        public string? X_Twitter { get; set; }

        //Time

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("closing_date")]
        public DateTime? ClosingDate { get; set; }





    }
}
