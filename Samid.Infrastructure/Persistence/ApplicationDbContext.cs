using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Samid.Domain.Entities;

namespace Samid.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
  public DbSet<AcademicYear> AcademicYears { get; set; }
  public DbSet<GradeOfStudy> GradeOfStudies { get; set; }
  public DbSet<FieldOfStudy> FieldOfStudies { get; set; }
  public DbSet<GradeFieldOfStudy> GradeFieldOfStudies { get; set; }

  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.Entity<GradeFieldOfStudy>()
      .HasKey(gfs => new { gfs.GradeOfStudyId, gfs.FieldOfStudyId });

    builder.Entity<GradeFieldOfStudy>()
      .HasOne(gfs => gfs.GradeOfStudy)
      .WithMany(g => g.GradeFields)
      .HasForeignKey(gfs => gfs.GradeOfStudyId);

    builder.Entity<GradeFieldOfStudy>()
      .HasOne(gfs => gfs.FieldOfStudy)
      .WithMany(f => f.GradeFields)
      .HasForeignKey(gfs => gfs.FieldOfStudyId);

  }
}