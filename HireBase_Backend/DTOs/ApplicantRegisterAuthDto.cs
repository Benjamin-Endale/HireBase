using System;
using System.ComponentModel.DataAnnotations;

namespace HireBase_Backend.DTOs
{
    public record ApplicantRegisterRequest
    {
        [Required]
        public string Fullname { get; init; } = null!;

        [Required, EmailAddress]
        public string Email { get; init; } = null!;

        [Required]
        public string Password { get; init; } = null!;
    }

    public record ApplicantLoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; init; } = null!;

        [Required]
        public string Password { get; init; } = null!;
    }

    public record ApplicantLoginResponse
    {
        public string? Id { get; init; } = null;
        public string? AccessToken { get; init; } = null;
        public DateTimeOffset? ExpiresAt { get; init; } = null;
        public string? RefreshToken { get; init; } = null;
        public DateTimeOffset? RefreshExpiresAt { get; init; } = null;
        public string? Fullname { get; init; } = null;
        public string? Email { get; init; } = null;
        public string? Message { get; init; } = null;
        public bool RequiresOtp { get; init; } = false;
        public bool OtpVerified { get; init; } = false;
    }

    public record OtpVerifyRequest
    {
        public string UsernameOrEmail { get; init; } = string.Empty;
        public string OtpCode { get; init; } = string.Empty;
    }
}
