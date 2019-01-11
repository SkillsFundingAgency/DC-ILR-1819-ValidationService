using System;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class AcademicYearQueryService : IAcademicYearQueryService
    {
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

        public DateTime AugustThirtyFirstOfLearnStartDate(DateTime dateLearnStartDate)
        {
            return new DateTime(dateLearnStartDate.Year, 08, 31);
        }

        public bool DateIsInPrevAcademicYear(DateTime dateTime, DateTime currentYear)
        {
            return dateTime < currentYear && dateTime >= currentYear.AddYears(-1);
        }

        public DateTime FirstAugustForDateInAcademicYear(DateTime dateTime)
        {
            return dateTime.Month > 8 ? new DateTime(dateTime.Year + 1, 8, 1) : new DateTime(dateTime.Year, 8, 1);
        }
    }
}
