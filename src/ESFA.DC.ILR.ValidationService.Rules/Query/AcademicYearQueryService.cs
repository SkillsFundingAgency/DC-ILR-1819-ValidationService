using System;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class AcademicYearQueryService : IAcademicYearQueryService
    {
        public DateTime FirstSeptemberForDateInAcademicYear(DateTime dateTime)
        {
            return dateTime.Month > 8 ? new DateTime(dateTime.Year, 9, 1) : new DateTime(dateTime.Year - 1, 9, 1);
        }

        public DateTime LastFridayInJuneForDateInAcademicYear(DateTime dateTime)
        {
            return LastFridayInMonth(dateTime.Month > 8 ? new DateTime(dateTime.Year + 1, 6, 1) : new DateTime(dateTime.Year, 6, 1));
        }

        public DateTime LastFridayInMonth(DateTime dateTime)
        {
            var firstDayOfNextMonth = new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1);

            var vector = (((int)firstDayOfNextMonth.DayOfWeek + 1) % 7) + 1;

            return firstDayOfNextMonth.AddDays(-vector);
        }

        public bool DateIsInPrevAcademicYear(DateTime dateTime, DateTime currentYear)
        {
            return dateTime < currentYear && dateTime >= currentYear.AddYears(-1);
        }
    }
}
