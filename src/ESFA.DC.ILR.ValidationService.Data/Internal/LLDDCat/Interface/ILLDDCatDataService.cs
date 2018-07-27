using System;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.LLDDCat.Interface
{
    public interface ILLDDCatDataService
    {
        bool Exists(int llddCat);

        bool IsDateValidForLLDDCat(int llddCat, DateTime date);
    }
}
