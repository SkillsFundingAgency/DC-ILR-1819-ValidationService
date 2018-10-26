using System;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.QUALENT3.Interface
{
    public interface IQUALENT3DataService
    {
        bool Exists(string qualent3);

        bool IsLearnStartDateBeforeValidTo(string qualent3, DateTime learnStartDate);
    }
}
