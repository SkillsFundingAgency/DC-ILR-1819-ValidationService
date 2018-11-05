using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface
{
    /// <summary>
    /// the academic year date format helper
    /// </summary>
    public static class AcademicYearDateFormatHelper
    {
        /// <summary>
        /// The formats for the academic dates of interest
        /// </summary>
        private static readonly Dictionary<AcademicYearDates, string> _dateFormats = new Dictionary<AcademicYearDates, string>
        {
            [AcademicYearDates.Commencment] = "{0}-08-01",
            [AcademicYearDates.PreviousYearEnd] = "{0}-07-31",
            [AcademicYearDates.August31] = "{0}-08-31"
        };

        /// <summary>
        /// Gets the date format.
        /// </summary>
        /// <param name="forThisDate">For this date.</param>
        /// <returns>the string format (just 'add' the year)</returns>
        public static string GetDateFormat(this AcademicYearDates forThisDate)
        {
            return _dateFormats[forThisDate];
        }
    }
}
