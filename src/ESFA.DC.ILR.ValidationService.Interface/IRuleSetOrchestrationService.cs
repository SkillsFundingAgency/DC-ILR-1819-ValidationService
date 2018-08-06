using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IRuleSetOrchestrationService<T, out U>
        where T : class
    {
        IEnumerable<U> Execute(IValidationContext validationContext);
    }
}
