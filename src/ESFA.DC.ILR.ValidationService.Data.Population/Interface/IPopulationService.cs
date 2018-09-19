using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IPopulationService
    {
        Task PopulateAsync(CancellationToken cancellationToken);
    }
}
