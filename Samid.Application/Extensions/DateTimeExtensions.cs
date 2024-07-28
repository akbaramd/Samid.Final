using System.Globalization;

namespace Samid.Application.Extensions;

public static class DateTimeExtensions
{
  public static string ToPersianDate(this DateTime date)
  {
    var persianCalendar = new PersianCalendar();
    var year = persianCalendar.GetYear(date);
    var month = persianCalendar.GetMonth(date);
    var day = persianCalendar.GetDayOfMonth(date);
    return $"{year}/{month:D2}/{day:D2}";
  }
}
