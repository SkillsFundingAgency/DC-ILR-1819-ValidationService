using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IInternalDataCachePopulationService : IPopulationService
    {
    }

    public interface IInternalDataCacheWithDataPopulationService : IPopulationService<IInternalDataCache>
    {
    }
}
