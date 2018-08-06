using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IPreValidationOrchestrationService<out U>
    {
        IEnumerable<U> Execute(IPreValidationContext validationContext);
    }
}