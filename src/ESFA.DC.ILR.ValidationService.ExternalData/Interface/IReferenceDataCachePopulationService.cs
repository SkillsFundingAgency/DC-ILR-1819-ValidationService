using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.ExternalData.Interface
{
    public interface IReferenceDataCachePopulationService<in T> where T : class
    {
        void Populate(IReferenceDataCache referenceDataCache, IEnumerable<T> validationItems);
    }
}
