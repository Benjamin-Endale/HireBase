using Microsoft.EntityFrameworkCore;
using HireBase_Backend.Models;

namespace HireBase_Backend.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options) { }

		// =======================
		// DbSets (JOB MODULE ONLY)
		// =======================

		public DbSet<Tenant> Tenants => Set<Tenant>();
		public DbSet<Job> Jobs => Set<Job>();

		public DbSet<ApplicantRegistration> ApplicantRegistrations => Set<ApplicantRegistration>();
		public DbSet<Applicant> Applicants => Set<Applicant>();
		public DbSet<ApplicantJob> ApplicantJobs => Set<ApplicantJob>();

		public DbSet<Shortlist> Shortlists => Set<Shortlist>();
		public DbSet<Interview> Interviews => Set<Interview>();

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();

        // =======================
        // Model Configuration
        // =======================

        protected override void OnModelCreating(ModelBuilder model)
		{
			base.OnModelCreating(model);

			// =======================
			// TENANT
			// =======================
			model.Entity<Tenant>(e =>
			{
				e.HasKey(t => t.Id);
				e.HasIndex(t => t.Domain).IsUnique();
			});

			// =======================
			// JOB
			// =======================
			model.Entity<Job>(e =>
			{
				e.HasKey(j => j.Id);

				e.Property(j => j.Id)
				 .HasDefaultValueSql("NEWSEQUENTIALID()");

				e.HasOne(j => j.Tenant)
				 .WithMany()
				 .HasForeignKey(j => j.TenantID)
				 .OnDelete(DeleteBehavior.Restrict);
			});

			// =======================
			// APPLICANT REGISTRATION
			// (Login / OTP / Profile)
			// =======================
			model.Entity<ApplicantRegistration>(e =>
			{
				e.HasKey(a => a.Id);

				e.HasIndex(a => a.Email)
				 .IsUnique();

				e.Property(a => a.Id)
				 .HasDefaultValueSql("NEWSEQUENTIALID()");
			});

			// =======================
			// APPLICANT (Applied job)
			// =======================
			model.Entity<Applicant>(e =>
			{
				e.HasKey(a => a.Id);

				e.Property(a => a.Id)
				 .HasDefaultValueSql("NEWSEQUENTIALID()");

				e.HasOne(a => a.Job)
				 .WithMany(j => j.Applicants)
				 .HasForeignKey(a => a.JobId)
				 .OnDelete(DeleteBehavior.Cascade);
			});

			// =======================
			// APPLICANT JOB
			// (Saved / Applied jobs)
			// =======================
			model.Entity<ApplicantJob>(e =>
			{
				e.HasKey(aj => aj.Id);

				e.HasOne(aj => aj.Applicant)
				 .WithMany()
				 .HasForeignKey(aj => aj.ApplicantId)
				 .OnDelete(DeleteBehavior.Cascade);

				e.HasOne(aj => aj.Job)
				 .WithMany()
				 .HasForeignKey(aj => aj.JobId)
				 .OnDelete(DeleteBehavior.Cascade);

				// Prevent duplicate save/apply
				e.HasIndex(aj => new { aj.ApplicantId, aj.JobId })
				 .IsUnique();
			});

			// =======================
			// SHORTLIST
			// =======================
			model.Entity<Shortlist>(e =>
			{
				e.HasKey(s => s.ShortlistID);

				e.HasOne(s => s.Job)
				 .WithMany()
				 .HasForeignKey(s => s.JobID)
				 .OnDelete(DeleteBehavior.Cascade);
			});

			// =======================
			// INTERVIEW
			// =======================
			model.Entity<Interview>(e =>
			{
				e.HasKey(i => i.Id);

				e.Property(i => i.Id)
				 .HasDefaultValueSql("NEWSEQUENTIALID()");

				e.HasOne(i => i.Shortlist)
				 .WithMany()
				 .HasForeignKey(i => i.ShortlistId)
				 .OnDelete(DeleteBehavior.Cascade);
			});

            // ===== USER ROLES RELATIONSHIP =====
            model.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            model.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);


            // ===== SEED SYSTEM ROLES =====
            // Predefined IDs for consistency
            var ROLE_SUPERADMIN = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var ROLE_SYSTEM_ADMIN = Guid.Parse("00000000-0000-0000-0000-000000000002");

            // Seed roles
            model.Entity<Role>().HasData(
                new Role
                {
                    Id = ROLE_SUPERADMIN,
                    Name = "SuperAdmin",
                    Description = "Full system access",
                    IsSystem = true,
                    PermissionsJson = "[\"*\"]"
                },
                new Role
                {
                    Id = ROLE_SYSTEM_ADMIN,
                    Name = "SystemAdmin",
                    Description = "Tenant admin access",
                    IsSystem = true,
                    PermissionsJson = "[\"AdminPage\"]"
                }
            );

            model.Entity<Employee>(entity =>
            {
                entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
                entity.Property(e => e.LeaveCredit).HasColumnType("decimal(5,2)");
            });

        }
    }
}
