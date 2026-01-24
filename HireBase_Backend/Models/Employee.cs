using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HireBase_Backend.Models
{
    [Table("employees")]
    public class Employee
    {
        [Key]
        [Column("id")]
        public Guid EmployeeID { get; set; } = Guid.NewGuid();

        [Required]
        [Column("tenant_id")]
        public Guid TenantId { get; set; }

        // Personal info
        [Required, MaxLength(100)]
        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [Column("gender")]
        public string Gender { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Column("nationality")]
        public string Nationality { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [Column("marital_status")]
        public string MaritalStatus { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        [Column("address")]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Column("date_of_birth")]
        public DateTime DateOfBirth { get; set; }

        // Contact
        [Required, MaxLength(255)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [Column("phone_number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        [Column("emergency_contact_name")]
        public string EmergencyContactName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [Column("emergency_contact_number")]
        public string EmergencyContactNumber { get; set; } = string.Empty;

        // Job
        [Required, MaxLength(150)]
        [Column("job_title")]
        public string JobTitle { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Column("employee_education_status")]
        public string EmployeeEducationStatus { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [Column("employee_type")]
        public string EmploymentType { get; set; } = string.Empty;

        [Required, MaxLength(2083)]
        [Column("photo_url")]
        public string PhotoUrl { get; set; } = string.Empty;

        [Required]
        [Column("hire_date")]
        public DateTime HireDate { get; set; }

        [MaxLength(50)]
        [Column("employee_code")]
        public string? EmployeeCode { get; set; } // required via controller rule but will be auto-generated if missing

        // Payroll / misc

        [Column("benefits_enrollment")]
        public string? BenefitsEnrollment { get; set; } // ← now nullable

        [Column("shift_details")]
        public string? ShiftDetails { get; set; }       // ← now nullable

        [Column("salary")]
        public decimal? Salary { get; set; }


        [Column("currency")]
        public string? Currency { get; set; }

        [Column("paymentMethod")]
        public string? PaymentMethod { get; set; }

        [Column("bankAccountNumber")]
        public string? BankAccountNumber { get; set; }

        [Column("taxIdenitificationNumber")]
        public string? TaxIdenitificationNumber { get; set; }

        [Column("passportNumber")]
        public string? PassportNumber { get; set; }

        [Column("resume")]
        public string? Resume { get; set; }

        [Column("contractFile")]
        public string? ContractFile { get; set; }

        [Column("workLocation")]
        public string? WorkLocation { get; set; }

        [Column("certification ")]
        public string? Certification { get; set; }

        // Timestamps
        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("terminated_date")]
        public DateTime? TerminatedDate { get; set; }

        [Column("leave_credit")]
        public decimal LeaveCredit { get; set; } = 20; // default 20 days
        public DateTime LastCreditUpdate { get; set; } = DateTime.UtcNow;


        // Navs
        public Tenant Tenant { get; set; } = null!;
    }
}
