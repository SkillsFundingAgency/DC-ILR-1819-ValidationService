using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationItemProviderService<T>
    {
        Task<T> ProvideAsync(CancellationToken cancellationToken);
    }
}
