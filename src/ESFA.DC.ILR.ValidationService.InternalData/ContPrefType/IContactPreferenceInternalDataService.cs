using System;

namespace ESFA.DC.ILR.ValidationService.InternalData.ContPrefType
{
    public interface IContactPreferenceInternalDataService
    {
        bool TypeExists(string type);

        bool CodeExists(long? code);

        bool TypeForCodeExist(string type, long? code, DateTime? validTo);
    }
}