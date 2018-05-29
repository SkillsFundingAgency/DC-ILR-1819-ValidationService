using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IExternalDataCachePopulationService : IPopulationService
    {
    }

    public interface IExternalDataCacheWithDataPopulationService : IPopulationService<IExternalDataCache>
    {
    }
}