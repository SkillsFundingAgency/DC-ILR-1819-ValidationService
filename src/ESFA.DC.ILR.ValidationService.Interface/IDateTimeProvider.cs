using System;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}
