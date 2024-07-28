using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Samid.Domain.Entities;

// Entity for AcademicYear
public class AcademicYear
{

  public AcademicYear()
  {
    StartDate = GetStartOfAcademicYear();
    EndDate = GetEndOfAcademicYear();
    Title = GenerateAcademicYearTitle(StartDate, EndDate);
  }

  [Key] public Guid Id { get; set; } // کلید اصلی

  [Required] public string Title { get; private set; } = string.Empty;

  [Required] public DateTime StartDate { get; private set; }

  [Required] public DateTime EndDate { get; private set; }

  public bool IsCurrentAcademicYear(DateTime date)
  {
    return date >= StartDate && date <= EndDate;
  }

  private string GenerateAcademicYearTitle(DateTime startDate, DateTime endDate)
  {
    var persianCalendar = new PersianCalendar();
    int startYear = persianCalendar.GetYear(startDate);
    int endYear = persianCalendar.GetYear(endDate);
    return $"سال تحصیلی {startYear}-{endYear}";
  }

  private DateTime GetStartOfAcademicYear()
  {
    var persianCalendar = new PersianCalendar();
    DateTime now = DateTime.Now;
    int year = persianCalendar.GetYear(now);
    int month = persianCalendar.GetMonth(now);

    if (month >= 7) // اگر ماه مهر یا بعد از آن باشد
    {
      return persianCalendar.ToDateTime(year, 7, 1, 0, 0, 0, 0); // 1 مهر همین سال
    }
    else
    {
      return persianCalendar.ToDateTime(year - 1, 7, 1, 0, 0, 0, 0); // 1 مهر سال قبل
    }
  }

  private DateTime GetEndOfAcademicYear()
  {
    var persianCalendar = new PersianCalendar();
    DateTime startOfAcademicYear = GetStartOfAcademicYear();
    int year = persianCalendar.GetYear(startOfAcademicYear);

    return persianCalendar.ToDateTime(year + 1, 6, 31, 23, 59, 59, 999); // 31 شهریور سال بعد
  }
}