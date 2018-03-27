using System;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IAcademicYearCalendarService
    {
        DateTime FirstSeptemberForDateInAcademicYear(DateTime dateTime);
        DateTime LastFridayInJuneForDateInAcademicYear(DateTime dateTime);
    }
}
