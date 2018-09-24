using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface
{
    public interface IProvideEPAOrganisationDetails
    {
        /// <summary>
        /// Determines whether the specified candidate identifier is known.
        /// </summary>
        /// <param name="candidateID">The candidate identifier.</param>
        /// <returns>
        ///   <c>true</c> if the specified candidate identifier is known; otherwise, <c>false</c>.
        /// </returns>
        bool IsKnown(string candidateID);

        /// <summary>
        /// Determines whether the specified candidate identifier is current.
        /// </summary>
        /// <param name="candidateID">The candidate identifier.</param>
        /// <param name="referenceDate">The reference date.</param>
        /// <returns>
        ///   <c>true</c> if the specified candidate identifier is current; otherwise, <c>false</c>.
        /// </returns>
        bool IsCurrent(string candidateID, DateTime referenceDate);
    }
}
