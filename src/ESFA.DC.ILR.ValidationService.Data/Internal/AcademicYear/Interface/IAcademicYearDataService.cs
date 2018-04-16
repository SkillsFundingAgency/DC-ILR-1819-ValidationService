using System;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface
{
    public interface IAcademicYearDataService
    {
        DateTime AugustThirtyFirst();

        DateTime End();

        DateTime JanuaryFirst();

        DateTime Start();
    }
}