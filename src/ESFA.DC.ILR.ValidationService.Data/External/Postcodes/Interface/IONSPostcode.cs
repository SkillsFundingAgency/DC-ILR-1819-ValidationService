using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface
{
    public interface IONSPostcode
    {
        /// <summary>
        /// Gets the postcode.
        /// </summary>
        string Postcode { get; }

        DateTime? Termination { get; }

        /// <summary>
        /// Gets the local authority.
        /// </summary>
        string LocalAuthority { get; }

        /// <summary>
        /// Gets the lep1.
        /// </summary>
        string Lep1 { get; }

        /// <summary>
        /// Gets the lep2.
        /// </summary>
        string Lep2 { get; }

        /// <summary>
        /// Gets the effective from (date).
        /// </summary>
        DateTime EffectiveFrom { get; }

        /// <summary>
        /// Gets the effective to.
        /// </summary>
        DateTime? EffectiveTo { get; }

        /// <summary>
        /// Gets the nuts (??).
        /// </summary>
        string Nuts { get; }
    }
}
