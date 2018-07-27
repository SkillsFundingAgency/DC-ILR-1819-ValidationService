using System;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.Model
{
    public struct ValidityPeriods
    {
        public DateTime ValidFrom;

        public DateTime ValidTo;

        public ValidityPeriods(DateTime validFrom, DateTime validTo)
        {
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
