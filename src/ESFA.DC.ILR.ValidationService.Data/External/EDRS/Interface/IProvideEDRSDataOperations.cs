using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.EDRS.Interface
{
    /// <summary>
    /// i provide EDRS data operations definition
    /// </summary>
    public interface IProvideEDRSDataOperations
    {
        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <param name="thisEmployer">this employer (reference number).</param>
        /// <returns>
        ///   <c>true</c> if the specified this employer is valid; otherwise, <c>false</c>.
        /// </returns>
        bool IsValid(int thisEmployer);

        /// <summary>
        /// Determines whether the specified this employer is temporary.
        /// </summary>
        /// <param name="thisEmployer">this employer (reference number).</param>
        /// <returns>
        ///   <c>true</c> if the specified this employer is temporary; otherwise, <c>false</c>.
        /// </returns>
        bool IsTemporary(int thisEmployer);
    }
}
