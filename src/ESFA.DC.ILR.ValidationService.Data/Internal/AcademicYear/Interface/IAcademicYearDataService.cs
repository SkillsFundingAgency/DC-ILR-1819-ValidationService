using System;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface
{
    /// <summary>
    /// the academic year data service
    /// </summary>
    public interface IAcademicYearDataService
    {
        DateTime AugustThirtyFirst();

        /// <summary>
        /// the academic year end.
        /// </summary>
        /// <returns>an academic year end date</returns>
        DateTime End();

        DateTime JanuaryFirst();

        DateTime JulyThirtyFirst();

        /// <summary>
        /// the academic year start.
        /// </summary>
        /// <returns>an academic year start date</returns>
        DateTime Start();
    }
}