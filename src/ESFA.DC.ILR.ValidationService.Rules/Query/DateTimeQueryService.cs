using System;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class DateTimeQueryService : IDateTimeQueryService
    {
        private const double DaysInYear = 365.242199;

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

        public int AgeAtGivenDate(DateTime dateOfBirth, DateTime givenDate)
        {
            return Convert.ToInt32(Math.Floor((givenDate - dateOfBirth).TotalDays / DaysInYear));
        }
    }
}
