using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HireBase_Backend.DTOs
{
    public sealed class EmployeeCreateDto
    {
        // Keys
        [Required]
        public Guid TenantId { get; set; }

        // Personal
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required, MaxLength(50)]
        public string Gender { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string Nationality { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string MaritalStatus { get; set; } = string.Empty;

        // Contact
        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required, MaxLength(500)]
        public string Address { get; set; } = string.Empty;
        [Required, MaxLength(200)]
        public string EmergencyContactName { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string EmergencyContactNumber { get; set; } = string.Empty;

        // Job
        [Required, MaxLength(150)]
        public string JobTitle { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string EmploymentType { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string EmployeeEducationStatus { get; set; } = string.Empty;
        [Required]
        public DateTime HireDate { get; set; }

        // Codes
        [MaxLength(50)]
        public string? EmployeeCode { get; set; }

        // Financial / misc
        public string? BenefitsEnrollment { get; set; }
        public string? ShiftDetails { get; set; }
        public decimal LeaveCredit { get; set; } = 20;
        public decimal? Salary { get; set; }
        public string? Currency { get; set; }
        public string? PaymentMethod { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? TaxIdenitificationNumber { get; set; }
        public string? PassportNumber { get; set; }
        public string? WorkLocation { get; set; }

        // Files
        public IFormFile? Photo { get; set; }
        public IFormFile? ResumeFile { get; set; }
        public IFormFile? ContractFile { get; set; }
        public IFormFile? CertificationFile { get; set; }
    }

    public sealed class EmployeeUpdateDto
    {
        [Required]
        public Guid EmployeeId { get; set; }
        [Required]
        public Guid TenantId { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required, MaxLength(50)]
        public string Gender { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string Nationality { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string MaritalStatus { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required, MaxLength(500)]
        public string Address { get; set; } = string.Empty;
        [Required, MaxLength(200)]
        public string EmergencyContactName { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string EmergencyContactNumber { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string JobTitle { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string EmploymentType { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string EmployeeEducationStatus { get; set; } = string.Empty;
        [Required]
        public DateTime HireDate { get; set; }

        [MaxLength(50)]
        public string? EmployeeCode { get; set; }

        public string? BenefitsEnrollment { get; set; }
        public string? ShiftDetails { get; set; }
        public decimal? Salary { get; set; }
        public string? Currency { get; set; }
        public string? PaymentMethod { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? TaxIdenitificationNumber { get; set; }
        public string? PassportNumber { get; set; }
        public string? WorkLocation { get; set; }

        // Files
        public IFormFile? Photo { get; set; }
        public IFormFile? ResumeFile { get; set; }
        public IFormFile? ContractFile { get; set; }
        public IFormFile? CertificationFile { get; set; }
    }

    public class EmployeeListDto
    {
        public Guid EmployeeID { get; set; }
        public Guid TenantId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? EmployeeCode { get; set; }
        public string? JobTitle { get; set; }
    }

    public class EmployeeDetailDto
    {
        public Guid EmployeeID { get; set; }
        public Guid TenantId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? EmployeeCode { get; set; }
        public string? JobTitle { get; set; }
    }
}
