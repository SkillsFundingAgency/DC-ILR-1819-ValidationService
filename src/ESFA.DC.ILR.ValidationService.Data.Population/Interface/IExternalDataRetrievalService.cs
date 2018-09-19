using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IExternalDataRetrievalService<TOut>
    {
        Task<TOut> RetrieveAsync(CancellationToken cancellationToken);
    }
}
