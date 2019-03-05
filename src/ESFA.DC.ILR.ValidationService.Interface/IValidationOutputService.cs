using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationOutputService
    {
        Task ProcessAsync(CancellationToken cancellationToken);
    }
}
