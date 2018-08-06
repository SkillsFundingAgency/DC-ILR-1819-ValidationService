using System;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }
    }
}
