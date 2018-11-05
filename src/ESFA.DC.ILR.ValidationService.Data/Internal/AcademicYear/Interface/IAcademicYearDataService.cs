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

        /// <summary>
        /// Gets an academic year of learning date.
        /// </summary>
        /// <param name="candidate">
        /// The candidate, representing the learning delivery start date
        /// or any other date where the commencement of the acadameic year
        /// for the date in question is required</param>
        /// <param name="yearDate">The academic year date (of interest)</param>
        /// <returns>
        /// an academic date for the year of learning
        /// </returns>
        DateTime GetAcademicYearOfLearningDate(DateTime candidate, AcademicYearDates yearDate);
    }
}