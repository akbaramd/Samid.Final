using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Samid.Domain.Entities;

namespace Samid.Infrastructure.Persistence;

public static class ApplicationDbContextSeed
{
  public static void Seed(ApplicationDbContext context, UserManager<User> userManager)
  {
    if (!context.EducationStages.Any())
    {
      var jsonData = File.ReadAllText("D:\\Projects\\Samid.backend\\Samid.Infrastructure\\stages.json");
      var stages = JsonConvert.DeserializeObject<List<StageDto>>(jsonData);

      if (stages != null)
      {
        foreach (var stageDto in stages)
        {
          var stage = new EducationStage(Guid.NewGuid(), stageDto.StageName);
          context.EducationStages.Add(stage);
          context.SaveChanges();

          foreach (var gradeDto in stageDto.Grades)
          {
            var grade = new EducationGrade(Guid.NewGuid(), gradeDto.GradeName, stage.Id);
            context.EducationGrades.Add(grade);
            context.SaveChanges();

            foreach (var fieldDto in gradeDto.Fields)
            {
              var field = context.EducationFields.FirstOrDefault(m => m.Title == fieldDto.FieldName) ??
                          new EducationField(Guid.NewGuid(), fieldDto.FieldName);

              if (!context.EducationFields.Any(m => m.Id == field.Id))
              {
                context.EducationFields.Add(field);
                context.SaveChanges();
              }

              var educationMajor =
                context.EducationMajors.FirstOrDefault(sm =>
                  sm.EducationGradeId == grade.Id && sm.EducationFieldId == field.Id) ??
                new EducationMajors($"{gradeDto.GradeName} - {stageDto.StageName} ({fieldDto.FieldName})", grade.Id, grade, field.Id, field);

              if (!context.EducationMajors.Any(sm => sm.Id == educationMajor.Id))
              {
                context.EducationMajors.Add(educationMajor);
                context.SaveChanges();
              }

              foreach (var bookDto in fieldDto.Books)
              {
                var book = context.EducationBooks.FirstOrDefault(b => b.Code == bookDto.BookCode) ??
                           new EducationBook(Guid.NewGuid(), bookDto.BookName, bookDto.BookCode);

                if (!context.EducationBooks.Any(b => b.Id == book.Id))
                {
                  context.EducationBooks.Add(book);
                  context.SaveChanges();
                }

                if (!educationMajor.EducationBooks.Contains(book))
                {
                  educationMajor.EducationBooks.Add(book);
                }
              }

              context.SaveChanges();
            }
          }
        }
      }
    }

    SeedUser(context, userManager);
  }

  private static void SeedUser(ApplicationDbContext context, UserManager<User> userManager)
  {
    var user = userManager.FindByNameAsync("09371770774").Result;
    if (user == null)
    {
      user = new User("Akbar", "Ahmadi", new DateTime(1996, 9, 8))
      {
        UserName = "09371770774", Email = "akbar.ahmadi@example.com", PhoneNumber = "09371770774"
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
      var isUpdated = false;

      if (user.FirstName != "Akbar")
      {
        if (user.LastName != null)
        {
          user.UpdateName("Akbar", user.LastName);
        }

        isUpdated = true;
      }

      if (user.LastName != "Ahmadi")
      {
        if (user.FirstName != null)
        {
          user.UpdateName(user.FirstName, "Ahmadi");
        }

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
    var konkoorTajrobi = context.EducationMajors
      .FirstOrDefault(sm => sm.Title.Equals("کنکور - متوسطه دوم (تجربی)"));

    var academicYear =
      context.AcademicYears.FirstOrDefault(x => x.StartDate <= DateTime.UtcNow && x.EndDate >= DateTime.Now);

    if (academicYear is null)
    {
      academicYear = context.AcademicYears.Add(new AcademicYear()).Entity;
      context.SaveChanges();
    }

    if (konkoorTajrobi != null)
    {
      var academicYearExists = context.UserEducationMajors
        .Any(ay => ay.UserId == user.Id && ay.EducationMajorsId == konkoorTajrobi.Id);

      if (!academicYearExists)
      {
        var userAcademicYear = new UserEducationMajors(user.Id, konkoorTajrobi.Id, academicYear.Id);
        context.UserEducationMajors.Add(userAcademicYear);
        context.SaveChanges();
      }
    }
  }
}

public class StageDto
{
  public string StageName { get; set; } = default!;
  public List<GradeDto> Grades { get; set; } = new List<GradeDto>();
}

public class GradeDto
{
  public string GradeName { get; set; } = default!;
  public List<FieldDto> Fields { get; set; } = new List<FieldDto>();
}

public class FieldDto
{
  public string FieldName { get; set; } = default!;
  public List<BookDto> Books { get; set; } = new List<BookDto>();
}

public class BookDto
{
  public string BookName { get; set; } = default!;
  public string BookCode { get; set; } = default!;
}