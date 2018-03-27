using System;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface IDateTimeQueryService
    {
        int YearsBetween(DateTime start, DateTime end);
    }
}
