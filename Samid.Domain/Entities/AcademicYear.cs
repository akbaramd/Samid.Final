using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Samid.Domain.Entities
{
  public class AcademicYear
  {
    public AcademicYear()
    {
      StartDate = GetStartOfAcademicYear();
      EndDate = GetEndOfAcademicYear();
      Title = GenerateAcademicYearTitle(StartDate, EndDate);
    }

    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Title { get; private set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public bool IsCurrentAcademicYear(DateTime date)
    {
      return date >= StartDate && date <= EndDate;
    }

    private string GenerateAcademicYearTitle(DateTime startDate, DateTime endDate)
    {
      var persianCalendar = new PersianCalendar();
      var startYear = persianCalendar.GetYear(startDate);
      var endYear = persianCalendar.GetYear(endDate);
      return $"سال تحصیلی {startYear}-{endYear}";
    }

    private DateTime GetStartOfAcademicYear()
    {
      var persianCalendar = new PersianCalendar();
      var now = DateTime.Now;
      var year = persianCalendar.GetYear(now);
      var month = persianCalendar.GetMonth(now);

      if (month >= 7)
      {
        return persianCalendar.ToDateTime(year, 7, 1, 0, 0, 0, 0);
      }

      return persianCalendar.ToDateTime(year - 1, 7, 1, 0, 0, 0, 0);
    }

    private DateTime GetEndOfAcademicYear()
    {
      var persianCalendar = new PersianCalendar();
      var startOfAcademicYear = GetStartOfAcademicYear();
      var year = persianCalendar.GetYear(startOfAcademicYear);

      return persianCalendar.ToDateTime(year + 1, 6, 31, 23, 59, 59, 999);
    }
  }
}
