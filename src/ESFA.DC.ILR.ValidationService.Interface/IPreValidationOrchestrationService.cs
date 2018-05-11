using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IPreValidationOrchestrationService<T, out U> where T : class
    {
        IEnumerable<U> Execute(IPreValidationContext validationContext);
    }
}