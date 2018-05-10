using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface ILearnerPerActorService<T, out U>
        where T : class
    {
        IEnumerable<U> Process();
    }
}