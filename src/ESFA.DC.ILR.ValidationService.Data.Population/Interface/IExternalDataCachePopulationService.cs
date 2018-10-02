using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IExternalDataCachePopulationService : IPopulationService
    {
        Task PopulateErrorLookupsAsync(CancellationToken cancellationToken);
    }
}