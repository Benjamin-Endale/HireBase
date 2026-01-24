
using System;

namespace HireBase_Backend.DTOs
{
    // Read model
    public class TenantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string? Industry { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Country { get; set; } = string.Empty;
        public string TimeZone { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
    }

    // Create model
    public class CreateTenantDto
    {
        public string Name { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string? Industry { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string Country { get; set; } = string.Empty;
        public string TimeZone { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
    }

    // Update model
    public class UpdateTenantDto : CreateTenantDto
    {
        public Guid Id { get; set; }
    }
}
