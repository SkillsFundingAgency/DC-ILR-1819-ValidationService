using System;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class DateTimeQueryService : IDateTimeQueryService
    {
        public int YearsBetween(DateTime start, DateTime end)
        {
            var years = end.Year - start.Year;

            return end < start.AddYears(years) ? years - 1 : years;
        }

        public int MonthsBetween(DateTime start, DateTime end)
        {
            int monthsApart = (12 * (start.Year - end.Year)) + start.Month - end.Month;

            return Math.Abs(monthsApart);
        }

        public double DaysBetween(DateTime start, DateTime end)
        {
            return (end - start).TotalDays;
        }

        public DateTime DateAddYears(DateTime date, int yearsToAdd)
        {
            return date.AddYears(yearsToAdd);
        }
    }
}
