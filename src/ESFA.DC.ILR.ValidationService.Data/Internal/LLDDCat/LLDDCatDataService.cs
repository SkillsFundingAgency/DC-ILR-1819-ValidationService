using System;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.LLDDCat.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.LLDDCat
{
    public class LLDDCatDataService : ILLDDCatDataService
    {
        private readonly IInternalDataCache _internalDataCache;

        public LLDDCatDataService(IInternalDataCache internalDataCache)
        {
            _internalDataCache = internalDataCache;
        }

        public bool Exists(int llddCat)
        {
            return _internalDataCache.LLDDCats.ContainsKey(llddCat);
        }

        public bool IsDateValidForLLDDCat(int llddCat, DateTime date)
        {
            if (Exists(llddCat))
            {
                var validityPeriods =
                    _internalDataCache.LLDDCats
                    .Where(k => k.Key == llddCat)
                    .Select(v => v.Value).Single();

                return validityPeriods.ValidFrom <= date
                    && validityPeriods.ValidTo >= date;
            }

            return false;
        }
    }
}
