using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.EDRS.Interface
{
    public interface IEmployersDataService
    {
        bool IsValid(int? empId);
    }
}
