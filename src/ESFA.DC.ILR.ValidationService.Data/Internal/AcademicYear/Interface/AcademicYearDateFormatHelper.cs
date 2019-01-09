using System;
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
            [AcademicYearDates.August31] = "{0}-08-31",
            [AcademicYearDates.CurrentYearEnd] = "{0}-07-31",
            [AcademicYearDates.NextYearCommencement] = "{0}-08-01",
        };

        private static readonly Dictionary<AcademicYearDates, Func<DateTime, int>> _dateActions = new Dictionary<AcademicYearDates, Func<DateTime, int>>
        {
            [AcademicYearDates.Commencment] = x => x.Month > 8 ? x.Year : x.Year - 1,
            [AcademicYearDates.PreviousYearEnd] = x => x.Month > 8 ? x.Year : x.Year - 1,
            [AcademicYearDates.August31] = x => x.Month > 8 ? x.Year : x.Year - 1,
            [AcademicYearDates.CurrentYearEnd] = x => x.Month < 8 ? x.Year : x.Year + 1,
            [AcademicYearDates.NextYearCommencement] = x => x.Month < 8 ? x.Year : x.Year + 1,
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

        /// <summary>
        /// Gets the date year.
        /// performs a selective shift on the candidates year number
        /// </summary>
        /// <param name="forThisDate">For this date.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>the shifted year</returns>
        public static int GetDateYear(this AcademicYearDates forThisDate, DateTime candidate)
        {
            return _dateActions[forThisDate](candidate);
        }
    }
}
