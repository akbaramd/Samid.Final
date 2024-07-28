using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Samid.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Samid.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static void Seed(ApplicationDbContext context, UserManager<User> userManager)
        {
            if (!context.GradeOfStudies.Any())
            {
                var grades = new[]
                {
                    new GradeOfStudy(Guid.NewGuid(), "اول ابتدایی"),
                    new GradeOfStudy(Guid.NewGuid(), "دوم ابتدایی"),
                    new GradeOfStudy(Guid.NewGuid(), "سوم ابتدایی"),
                    new GradeOfStudy(Guid.NewGuid(), "چهارم ابتدایی"),
                    new GradeOfStudy(Guid.NewGuid(), "پنجم ابتدایی"),
                    new GradeOfStudy(Guid.NewGuid(), "ششم ابتدایی"),
                    new GradeOfStudy(Guid.NewGuid(), "اول راهنمایی"),
                    new GradeOfStudy(Guid.NewGuid(), "دوم راهنمایی"),
                    new GradeOfStudy(Guid.NewGuid(), "سوم راهنمایی"),
                    new GradeOfStudy(Guid.NewGuid(), "اول دبیرستان"),
                    new GradeOfStudy(Guid.NewGuid(), "دوم دبیرستان"),
                    new GradeOfStudy(Guid.NewGuid(), "سوم دبیرستان"),
                    new GradeOfStudy(Guid.NewGuid(), "پیش دانشگاهی"),
                    new GradeOfStudy(Guid.NewGuid(), "کنکور")
                };

                context.GradeOfStudies.AddRange(grades);
                context.SaveChanges();
            }

            if (!context.FieldOfStudies.Any())
            {
                var generalField = new FieldOfStudy(Guid.NewGuid(), "عمومی");

                var fieldOfStudies = new[]
                {
                    generalField,
                    new FieldOfStudy(Guid.NewGuid(), "ریاضی و فیزیک"),
                    new FieldOfStudy(Guid.NewGuid(), "علوم تجربی"),
                    new FieldOfStudy(Guid.NewGuid(), "علوم انسانی"),
                    new FieldOfStudy(Guid.NewGuid(), "فنی و حرفه‌ای")
                };

                context.FieldOfStudies.AddRange(fieldOfStudies);
                context.SaveChanges();

                // Get the "فنی و حرفه‌ای" field to add its subfields
                var technicalField = context.FieldOfStudies.FirstOrDefault(f => f.Title == "فنی و حرفه‌ای");

                if (technicalField != null)
                {
                    var subFields = new[]
                    {
                        new FieldOfStudy(Guid.NewGuid(), "هنر", technicalField),
                        new FieldOfStudy(Guid.NewGuid(), "مکانیک", technicalField),
                        new FieldOfStudy(Guid.NewGuid(), "کامپیوتر", technicalField),
                        new FieldOfStudy(Guid.NewGuid(), "برق", technicalField)
                    };

                    context.FieldOfStudies.AddRange(subFields);
                    context.SaveChanges();
                }

                // Seed GradeFieldOfStudy relationships for general grades
                var generalGrades = context.GradeOfStudies
                    .Where(g => g.Title.StartsWith("ابتدایی") || g.Title.StartsWith("راهنمایی") || g.Title == "اول دبیرستان")
                    .ToList();

                foreach (var grade in generalGrades)
                {
                    var gradeField = new GradeFieldOfStudy(grade.Title, grade.Id, grade, generalField.Id, generalField);
                    context.GradeFieldOfStudies.Add(gradeField);
                }

                // Seed GradeFieldOfStudy relationships for specialized fields for "دوم دبیرستان"، "سوم دبیرستان"، "پیش دانشگاهی" و "کنکور"
                var specializedGrades = context.GradeOfStudies
                    .Where(g => g.Title == "دوم دبیرستان" || g.Title == "سوم دبیرستان" || g.Title == "پیش دانشگاهی" || g.Title == "کنکور")
                    .ToList();

                var specializedFields = context.FieldOfStudies
                    .Where(f => f.ParentId == null && f.Title != "عمومی")
                    .ToList();

                foreach (var grade in specializedGrades)
                {
                    foreach (var field in specializedFields)
                    {
                        var gradeField = new GradeFieldOfStudy($"{grade.Title} {field.Title}", grade.Id, grade, field.Id, field);
                        context.GradeFieldOfStudies.Add(gradeField);
                    }
                }

                context.SaveChanges();
            }

            // Add or update the user Akbar Ahmadi
            var user = userManager.FindByNameAsync("akbar.ahmadi@example.com").Result;
            if (user == null)
            {
                user = new User("Akbar", "Ahmadi", new DateTime(1996, 9, 8))
                {
                    UserName = "akbar.ahmadi@example.com",
                    Email = "akbar.ahmadi@example.com",
                    PhoneNumber = "09371770774"
                };

                var result = userManager.CreateAsync(user, "SecurePassword123!").Result;

                if (result.Succeeded)
                {
                    // Optionally add the user to a role
                    // userManager.AddToRoleAsync(user, "UserRole").Wait();
                }
            }
            else
            {
                // Update user properties if they have changed
                bool isUpdated = false;

                if (user.FirstName != "Akbar")
                {
                    user.UpdateName("Akbar", user.LastName);
                    isUpdated = true;
                }
                if (user.LastName != "Ahmadi")
                {
                    user.UpdateName(user.FirstName, "Ahmadi");
                    isUpdated = true;
                }
                if (user.BirthDate != new DateTime(1996, 9, 8))
                {
                    user.UpdateBirthDate(new DateTime(1996, 9, 8));
                    isUpdated = true;
                }
                if (user.PhoneNumber != "09371770774")
                {
                    user.PhoneNumber = "09371770774";
                    isUpdated = true;
                }

                if (isUpdated)
                {
                    var result = userManager.UpdateAsync(user).Result;
                }
            }

            // Create AcademicYear for کنکور تجربی
            var konkoorTajrobi = context.GradeFieldOfStudies
                .FirstOrDefault(gfs => gfs.Title == "کنکور علوم تجربی");

            var academicYear = context.AcademicYears.FirstOrDefault(x => x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now);

            if (academicYear is null)
            {
                academicYear = (context.AcademicYears.Add(new AcademicYear()).Entity);
                context.SaveChanges();
            }
            
            if (konkoorTajrobi != null)
            {
                var academicYearExists = context.UserAcademicYears
                    .Any(ay => ay.UserId == user.Id && ay.GradeFieldOfStudyId == konkoorTajrobi.Id);

                if (!academicYearExists)
                {   
                    var userAcademicYear = new UserAcademicYear(user.Id, konkoorTajrobi.Id,academicYear.Id);
                    context.UserAcademicYears.Add(userAcademicYear);
                    context.SaveChanges();
                }
            }
        }
    }
}
