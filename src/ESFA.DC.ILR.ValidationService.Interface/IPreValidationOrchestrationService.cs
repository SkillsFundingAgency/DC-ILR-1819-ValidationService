using System.Collections.Generic;
using System.Threading;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IPreValidationOrchestrationService<out U>
    {
        IEnumerable<U> Execute(IPreValidationContext validationContext, CancellationToken cancellationToken);
    }
}