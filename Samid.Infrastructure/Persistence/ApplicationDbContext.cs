using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Samid.Domain.Entities;

namespace Samid.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserEducationMajors> UserEducationMajors { get; set; }
        public DbSet<AcademicYear> AcademicYears { get; set; }
        public DbSet<EducationGrade> EducationGrades { get; set; }
        public DbSet<EducationField> EducationFields { get; set; }
        public DbSet<EducationStage> EducationStages { get; set; }
        public DbSet<EducationMajors> EducationMajors { get; set; }
        public DbSet<EducationBook> EducationBooks { get; set; }
        public DbSet<StudyActivity> StudyActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasMany(x => x.UserEducationMajors)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);

            builder.Entity<EducationMajors>()
              .HasMany(sm => sm.EducationBooks)
              .WithMany(b => b.EducationMajors)
              .UsingEntity<Dictionary<string, object>>(
                "EducationMajorsBook",
                j => j
                  .HasOne<EducationBook>()
                  .WithMany()
                  .HasForeignKey("EducationBookId")
                  .HasConstraintName("FK_EducationMajorsBook_EducationBookId")
                  .OnDelete(DeleteBehavior.Cascade),
                j => j
                  .HasOne<EducationMajors>()
                  .WithMany()
                  .HasForeignKey("EducationMajorsId")
                  .HasConstraintName("FK_EducationMajorsBook_EducationMajorsId")
                  .OnDelete(DeleteBehavior.Cascade));

            builder.Entity<EducationGrade>()
                .HasMany(sg => sg.EducationFields)
                .WithMany(sf => sf.EducationGrades)
                .UsingEntity<Dictionary<string, object>>(
                    "EducationGradeField",
                    j => j
                        .HasOne<EducationField>()
                        .WithMany()
                        .HasForeignKey("EducationFieldId")
                        .HasConstraintName("FK_EducationGradeField_EducationFieldId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<EducationGrade>()
                        .WithMany()
                        .HasForeignKey("EducationGradeId")
                        .HasConstraintName("FK_EducationGradeField_EducationGradeId")
                        .OnDelete(DeleteBehavior.Cascade));
        }
    }
}
