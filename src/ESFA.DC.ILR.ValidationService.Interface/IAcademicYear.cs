using System;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IAcademicYear
    {
        DateTime AugustThirtyFirst { get; }

        DateTime End { get; }

        DateTime JanuaryFirst { get; }

        DateTime Start { get; }
    }
}
