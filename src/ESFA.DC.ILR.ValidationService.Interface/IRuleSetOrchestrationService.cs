using System.Collections.Generic;
using System.Threading;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IRuleSetOrchestrationService<T, out U>
        where T : class
    {
        IEnumerable<U> Execute(CancellationToken cancellationToken);
    }
}
