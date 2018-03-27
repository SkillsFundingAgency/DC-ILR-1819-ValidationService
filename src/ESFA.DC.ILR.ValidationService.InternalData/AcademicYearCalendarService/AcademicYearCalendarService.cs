using System;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.InternalData.AcademicYearCalendarService
{
    public class AcademicYearCalendarService : IAcademicYearCalendarService
    {
        public DateTime FirstSeptemberForDateInAcademicYear(DateTime dateTime)
        {
            if (dateTime.Month > 8)
            {
                return new DateTime(dateTime.Year, 9, 1);
            }

            return new DateTime(dateTime.Year - 1, 9, 1);
        }

        public DateTime LastFridayInJuneForDateInAcademicYear(DateTime dateTime)
        {
            if (dateTime.Month > 8)
            {
                return LastFridayInMonth(new DateTime(dateTime.Year + 1, 6, 1));
            }

            return LastFridayInMonth(new DateTime(dateTime.Year, 6, 1));
        }

        public DateTime LastFridayInMonth(DateTime dateTime)
        {
            var firstDayOfNextMonth = new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1);

            int vector = (((int)firstDayOfNextMonth.DayOfWeek + 1) % 7) + 1;

            return firstDayOfNextMonth.AddDays(-vector);
        }
    }
}
