using ESFA.DC.ILR.ValidationService.Data.File;
using ESFA.DC.ILR.ValidationService.Data.Internal;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IInternalDataCachePopulationService : IPopulationService
    {
    }

    public interface IInternalDataCacheWithDataPopulationService : IPopulationService<IInternalDataCache>
    {
    }
}
