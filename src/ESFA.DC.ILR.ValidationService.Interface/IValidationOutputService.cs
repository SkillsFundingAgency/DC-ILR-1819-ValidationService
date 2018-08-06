using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationOutputService<out T>
    {
        IEnumerable<T> Process();
    }
}
