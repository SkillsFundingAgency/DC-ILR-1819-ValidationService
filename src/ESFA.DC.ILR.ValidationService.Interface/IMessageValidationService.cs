using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IMessageValidationService<U>
    {
        IEnumerable<IValidationError> Execute(IMessage message);
    }
}
