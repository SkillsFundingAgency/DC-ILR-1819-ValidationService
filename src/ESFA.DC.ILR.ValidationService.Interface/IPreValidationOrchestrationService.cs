using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IPreValidationOrchestrationService<U>
    {
        Task<IEnumerable<U>> ExecuteAsync(IPreValidationContext validationContext, CancellationToken cancellationToken);
    }
}