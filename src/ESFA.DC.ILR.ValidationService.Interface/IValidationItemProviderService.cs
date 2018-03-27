using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationItemProviderService<out T> where T : class
    {
        IEnumerable<T> Provide(IValidationContext validationContext);
    }
}
