using System;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class AcademicYear : IAcademicYear
    {
        public DateTime AugustThirtyFirst { get; set; }

        public DateTime End { get; set; }

        public DateTime JanuaryFirst { get; set; }

        public DateTime Start { get; set; }
    }
}
