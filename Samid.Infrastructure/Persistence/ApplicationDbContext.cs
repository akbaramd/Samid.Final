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

        public DbSet<UserStudyMajors> UserStudyMajors { get; set; }
        public DbSet<AcademicYear> AcademicYears { get; set; }
        public DbSet<StudyGrade> StudyGrades { get; set; }
        public DbSet<StudyField> StudyFields { get; set; }
        public DbSet<StudyStage> StudyStages { get; set; }
        public DbSet<StudyMajors> StudyMajors { get; set; }
        public DbSet<StudyBook> StudyBooks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasMany(x => x.UserStudyMajors)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);

            builder.Entity<StudyMajors>()
              .HasMany(sm => sm.StudyBooks)
              .WithMany(b => b.StudyMajors)
              .UsingEntity<Dictionary<string, object>>(
                "StudyMajorsBook",
                j => j
                  .HasOne<StudyBook>()
                  .WithMany()
                  .HasForeignKey("StudyBookId")
                  .HasConstraintName("FK_StudyMajorsBook_StudyBookId")
                  .OnDelete(DeleteBehavior.Cascade),
                j => j
                  .HasOne<StudyMajors>()
                  .WithMany()
                  .HasForeignKey("StudyMajorsId")
                  .HasConstraintName("FK_StudyMajorsBook_StudyMajorsId")
                  .OnDelete(DeleteBehavior.Cascade));

            builder.Entity<StudyGrade>()
                .HasMany(sg => sg.StudyFields)
                .WithMany(sf => sf.StudyGrades)
                .UsingEntity<Dictionary<string, object>>(
                    "StudyGradeField",
                    j => j
                        .HasOne<StudyField>()
                        .WithMany()
                        .HasForeignKey("StudyFieldId")
                        .HasConstraintName("FK_StudyGradeField_StudyFieldId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<StudyGrade>()
                        .WithMany()
                        .HasForeignKey("StudyGradeId")
                        .HasConstraintName("FK_StudyGradeField_StudyGradeId")
                        .OnDelete(DeleteBehavior.Cascade));
        }
    }
}
