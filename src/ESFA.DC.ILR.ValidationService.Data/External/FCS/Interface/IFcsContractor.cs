using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the FCS Contractor definition
    /// </summary>
    public interface IFcsContractor :
        IIdentifiableItem
    {
        /// <summary>
        /// Gets the organisation identifier.
        /// </summary>
        string OrganisationID { get; }

        /// <summary>
        /// Gets the ukprn.
        /// </summary>
        int UKPRN { get; }

        /// <summary>
        /// Gets the legal name.
        /// </summary>
        string LegalName { get; }

        /// <summary>
        /// Gets the syndication item identifier.
        /// </summary>
        Guid SyndicationItemID { get; }
    }
}
