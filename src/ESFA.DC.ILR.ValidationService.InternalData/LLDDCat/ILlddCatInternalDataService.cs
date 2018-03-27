using System;

namespace ESFA.DC.ILR.ValidationService.InternalData.LLDDCat
{
    public interface ILlddCatInternalDataService
    {
        bool CategoryExists(long? category);

        bool CategoryExistForDate(long? category, DateTime? validTo);
    }
}