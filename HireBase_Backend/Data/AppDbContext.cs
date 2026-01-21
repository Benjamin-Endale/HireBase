using Microsoft.EntityFrameworkCore;
using HireBase_Backend.Models;

namespace HireBase_Backend.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options) { }

		public DbSet<Applicant> Applicants { get; set; }
		public DbSet<Job> Jobs { get; set; }
		public DbSet<Recruiter> Recruiters { get; set; }
		public DbSet<Shortlist> Shortlists { get; set; }
		public DbSet<Interview> Interviews { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// =========================
			// Recruiter -> Jobs
			// =========================
			modelBuilder.Entity<Job>()
				.HasOne(j => j.Recruiter)
				.WithMany(r => r.Jobs)
				.HasForeignKey(j => j.RecruiterID)
				.OnDelete(DeleteBehavior.Cascade);

			// =========================
			// Job -> Applicants
			// =========================
			modelBuilder.Entity<Applicant>()
				.HasOne(a => a.Job)
				.WithMany()
				.HasForeignKey(a => a.JobId)
				.OnDelete(DeleteBehavior.Cascade);

			// =========================
			// Recruiter -> Applicants
			// =========================
			modelBuilder.Entity<Applicant>()
				.HasOne(a => a.Recruiter)
				.WithMany(r => r.Applicants)
				.HasForeignKey(a => a.RecruiterId)
				.OnDelete(DeleteBehavior.Restrict);

			// =========================
			// Job -> Shortlists
			// =========================
			modelBuilder.Entity<Shortlist>()
				.HasOne(s => s.Job)
				.WithMany()
				.HasForeignKey(s => s.JobId)
				.OnDelete(DeleteBehavior.Cascade);

			// =========================
			// Recruiter -> Shortlists
			// =========================
			modelBuilder.Entity<Shortlist>()
				.HasOne(s => s.Recruiter)
				.WithMany()
				.HasForeignKey(s => s.RecruiterId)
				.OnDelete(DeleteBehavior.Restrict);

			// =========================
			// Shortlist -> Interviews
			// =========================
			modelBuilder.Entity<Interview>()
				.HasOne(i => i.Shortlist)
				.WithMany()
				.HasForeignKey(i => i.ShortlistId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
