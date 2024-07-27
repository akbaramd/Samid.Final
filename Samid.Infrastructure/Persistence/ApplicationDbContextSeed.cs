using System;
using System.Linq;
using Samid.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Samid.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static void Seed(ApplicationDbContext context)
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
                    var gradeField = new GradeFieldOfStudy(grade.Id, grade, generalField.Id, generalField);
                    context.GradeFieldOfStudies.Add(gradeField);
                }

                // Seed GradeFieldOfStudy relationships for specialized fields for "دوم دبیرستان"، "سوم دبیرستان"، "پیش دانشگاهی" و "کنکور"
                var specializedGrades = context.GradeOfStudies
                    .Where(g => g.Title == "دوم دبیرستان" || g.Title == "سوم دبیرستان" || g.Title == "پیش دانشگاهی" || g.Title == "کنکور")
                    .ToList();

                var specializedFields = context.FieldOfStudies
                    .Where(f => (f.ParentId == null || f.ParentId == Guid.Empty) && f.Title != "عمومی")
                    .ToList();

                foreach (var grade in specializedGrades)
                {
                    foreach (var field in specializedFields)
                    {
                        var gradeField = new GradeFieldOfStudy(grade.Id, grade, field.Id, field);
                        context.GradeFieldOfStudies.Add(gradeField);
                    }
                }

                context.SaveChanges();
            }
        }
    }
}
