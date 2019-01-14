using System;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface IAcademicYearQueryService
    {
        DateTime LastFridayInJuneForDateInAcademicYear(DateTime dateTime);

        bool DateIsInPrevAcademicYear(DateTime dateTime, DateTime currentYear);
    }
}
