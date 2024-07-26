using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Samid.Inrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    // Configure the User entity
    builder.Entity<User>(entity =>
    {
      entity.Property(e => e.FirstName)
        .HasMaxLength(100)
        .IsRequired(false);

      entity.Property(e => e.LastName)
        .HasMaxLength(100)
        .IsRequired(false);

      entity.Property(e => e.BirthDate)
        .IsRequired(false);
    });
  }
}