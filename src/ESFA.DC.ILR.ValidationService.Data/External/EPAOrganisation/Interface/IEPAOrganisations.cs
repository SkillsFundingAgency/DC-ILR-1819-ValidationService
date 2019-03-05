using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface
{
    public interface IEPAOrganisations
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        string ID { get; }

        /// <summary>
        /// Gets the standard.
        /// </summary>
        string Standard { get; }

        /// <summary>
        /// Gets the effective from (date).
        /// </summary>
        DateTime EffectiveFrom { get; }

        /// <summary>
        /// Gets the effective to (date).
        /// </summary>
        DateTime? EffectiveTo { get; }
    }
}
