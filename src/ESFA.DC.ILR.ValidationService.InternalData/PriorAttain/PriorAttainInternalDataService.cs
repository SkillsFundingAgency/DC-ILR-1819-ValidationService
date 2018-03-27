using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.InternalData.PriorAttain
{
    public class PriorAttainInternalDataService : IPriorAttainInternalDataService
    {
        private readonly HashSet<long> _validPriorAttainLookupValues = new HashSet<long> { 2, 3, 4, 5, 10, 11, 12, 13, 97, 98 };

        public bool Exists(long priorAttain)
        {
            return _validPriorAttainLookupValues.Contains(priorAttain);
        }
    }
}