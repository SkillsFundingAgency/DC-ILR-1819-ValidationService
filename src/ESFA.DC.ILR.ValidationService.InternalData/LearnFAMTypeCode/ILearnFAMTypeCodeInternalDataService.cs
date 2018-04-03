using System;

namespace ESFA.DC.ILR.ValidationService.InternalData.LearnFAMTypeCode
{
    public interface ILearnFAMTypeCodeInternalDataService
    {
        bool TypeExists(string type);

        bool TypeCodeExists(string type, long? code);

        bool TypeCodeForDateExists(string type, long? code, DateTime? validTo);
    }
}
