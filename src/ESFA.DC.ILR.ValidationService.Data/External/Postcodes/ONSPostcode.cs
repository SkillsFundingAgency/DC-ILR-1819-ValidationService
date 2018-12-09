using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.Postcodes
{
    /// <summary>
    /// the ONS postcode implementation
    /// </summary>
    /// <seealso cref="IONSPostcode" />
    public class ONSPostcode :
        IONSPostcode
    {
        /// <summary>
        /// Gets or sets the postcode.
        /// </summary>
        public string Postcode { get; set; }

        /// <summary>
        /// Gets or sets the introduction.
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        /// Gets or sets the termination <= this should be a date...
        /// </summary>
        public string Termination { get; set; }

        /// <summary>
        /// Gets or sets the local authority.
        /// </summary>
        public string LocalAuthority { get; set; }

        /// <summary>
        /// Gets or sets the lep1.
        /// </summary>
        public string Lep1 { get; set; }

        /// <summary>
        /// Gets or sets the lep2.
        /// </summary>
        public string Lep2 { get; set; }

        /// <summary>
        /// Gets or sets the effective from (date).
        /// </summary>
        public DateTime EffectiveFrom { get; set; }

        /// <summary>
        /// Gets or sets the effective to.
        /// </summary>
        public DateTime? EffectiveTo { get; set; }

        /// <summary>
        /// Gets or sets the nuts (??).
        /// </summary>
        public string Nuts { get; set; }
    }
}
