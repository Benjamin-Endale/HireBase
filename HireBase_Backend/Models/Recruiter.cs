using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HireBase_Backend.Models
{
    public class Recruiter
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Company title is required")]
        [MaxLength(255, ErrorMessage = "Company title cannot exceed 255 characters")]
        public string Company_Name { get; set; } = string.Empty;

        [Column("normalized_email"), MaxLength(255)]
        public string? NormalizedEmail { get; set; }

        [EmailAddress, MaxLength(255)]
        public string? Country { get; set; }

        [Column("password_hash"), Required]
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

        [Column("password_salt"), Required]
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        // Time

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
           
        [Column("closing_date")]
        public DateTime? ClosingDate { get; set; }

        // Navigations 

        public ICollection<Job> Jobs { get; set; } = new List<Job>();
        public ICollection<Applicant> Applicants { get; set; } = new List<Applicant>();




    }
}
