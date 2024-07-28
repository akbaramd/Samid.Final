using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Samid.Domain.Entities;

namespace Samid.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
  {
  }

  public DbSet<UserAcademicYear> UserAcademicYears { get; set; }
  public DbSet<AcademicYear> AcademicYears { get; set; }
  public DbSet<GradeOfStudy> GradeOfStudies { get; set; }
  public DbSet<FieldOfStudy> FieldOfStudies { get; set; }
  public DbSet<GradeFieldOfStudy> GradeFieldOfStudies { get; set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.Entity<User>().HasMany(x => x.UserAcademicYears).WithOne(x => x.User).HasForeignKey(x => x.UserId);
  }
}
