using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationOutputService<T>
    {
        Task<IEnumerable<T>> ProcessAsync(CancellationToken cancellationToken);
    }
}
