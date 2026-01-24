// Models/Tenant.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HireBase_Backend.Models
{
    [Table("tenants")]
    public class Tenant
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("tenant_name"), Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // NOW REQUIRED
        [Column("domain"), Required, MaxLength(255)]
        public string Domain { get; set; } = string.Empty;

        [Column("industry"), MaxLength(100)]
        public string? Industry { get; set; }

        [Column("location"), MaxLength(200)]
        public string? Location { get; set; }

        [Column("description"), MaxLength(400)]
        public string? Description { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string Country { get; set; } = string.Empty;
        public string TimeZone { get; set; } = string.Empty;



        [MaxLength(50)]
        public string Status { get; set; } = "Active";


    }
}
