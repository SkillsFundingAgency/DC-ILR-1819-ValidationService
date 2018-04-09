using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IExternalDataCachePopulationService<in T> where T : class
    {
        void Populate(IEnumerable<T> validationItems);
    }
}