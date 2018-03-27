using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationDataService
    {
        DateTime AcademicYearAugustThirtyFirst { get; }
        DateTime AcademicYearEnd { get; }
        DateTime AcademicYearJanuaryFirst { get; }
        DateTime AcademicYearStart { get; }
        IReadOnlyCollection<long> ApprenticeProgTypes { get; }
        DateTime ApprencticeProgAllowedStartDate { get; }
        DateTime ValidationStartDateTime { get; }
    }
}
