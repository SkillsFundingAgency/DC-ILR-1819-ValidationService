using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface;
using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Model
{
    /// <summary>
    /// the EPA organisation class
    /// </summary>
    /// <seealso cref="IEPAOrganisation" />
    public class EPAOrganisation :
        IEPAOrganisation
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the standard.
        /// </summary>
        public string Standard { get; set; }

        /// <summary>
        /// Gets or sets the effective from (date).
        /// </summary>
        public DateTime EffectiveFrom { get; set; }

        /// <summary>
        /// Gets or sets the effective to (date).
        /// </summary>
        public DateTime EffectiveTo { get; set; }
    }
}
