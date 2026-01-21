using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HireBase_Backend.Models
{
    [Table("jobs")]
    public class Job
    {

        // ID's

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Recruiter ID is required")]
        public Guid? RecruiterID { get; set; }
        public Recruiter? Recruiter { get; set; }

  

        [Required(ErrorMessage = "Job title is required")]
        [MaxLength(255, ErrorMessage = "Job title cannot exceed 255 characters")]
        public string JobTitle { get; set; } = string.Empty;

        [MaxLength(255, ErrorMessage = "Location cannot exceed 255 characters")]
        public string Location { get; set; } = string.Empty;

        [MaxLength(255, ErrorMessage = "Job position cannot exceed 255 characters")]
        public string Job_Position { get; set; } = string.Empty;

        [MaxLength(255, ErrorMessage = "Employee type cannot exceed 255 characters")]
        public string Employee_Type { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Salary cannot exceed 100 characters")]
        public string Salary { get; set; } = string.Empty;

        [Required(ErrorMessage = "About role is required")]
        public string About_Role { get; set; } = string.Empty;

        [Required(ErrorMessage = "Task is required")]
        public string Task { get; set; } = string.Empty;

        [Required(ErrorMessage = "Requirements is required")]
        public string Requirements { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("status")]
        public string? Status { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("closing_date")]
        public DateTime? ClosingDate { get; set; }



    }
}
