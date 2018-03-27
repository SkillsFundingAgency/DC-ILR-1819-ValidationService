using System;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class DateTimeQueryService : IDateTimeQueryService
    {
        public int YearsBetween(DateTime start, DateTime end)
        {
            int years = end.Year - start.Year;

            return end < start.AddYears(years) ? years - 1 : years;
        }
    }
}
