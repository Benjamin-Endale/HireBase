using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HireBase_Backend.Models
{
	[Table("applicant_registrations")]
	public class ApplicantRegistration
	{
		[Key]
		[Column("id")]
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required]
		[Column("fullname")]
		public string Fullname { get; set; } = string.Empty;  //  must exist

		[Required]
		[Column("email")]
		public string Email { get; set; } = string.Empty;

		[Required]
		[Column("password_hash")]
		public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

		[Required]
		[Column("password_salt")]
		public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();


		// 2FA fields
		public string? OtpCode { get; set; }
		public DateTime? OtpExpiryUtc { get; set; }

		public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
	}
}


