using System;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface IAcademicYearQueryService
    {
        DateTime FirstSeptemberForDateInAcademicYear(DateTime dateTime);

        DateTime LastFridayInJuneForDateInAcademicYear(DateTime dateTime);

        DateTime AugustThirtyFirstOfLearnStartDate(DateTime dateTime);
    }
}
