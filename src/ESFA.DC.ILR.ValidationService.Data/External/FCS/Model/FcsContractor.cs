using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Model
{
    /// <summary>
    /// the FCS contractor implementation
    /// </summary>
    /// <seealso cref="IFcsContractor" />
    public class FcsContractor :
        IFcsContractor
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the organisation identifier.
        /// </summary>
        public string OrganisationID { get; set; }

        /// <summary>
        /// Gets or sets the ukprn.
        /// </summary>
        public int UKPRN { get; set; }

        /// <summary>
        /// Gets or sets the legal name.
        /// </summary>
        public string LegalName { get; set; }

        /// <summary>
        /// Gets or sets the syndication item identifier.
        /// </summary>
        public Guid SyndicationItemID { get; set; }
    }
}
