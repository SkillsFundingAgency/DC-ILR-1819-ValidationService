using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the FCS Contract definition
    /// </summary>
    public interface IFcsContract :
        IIdentifiableItem
    {
        /// <summary>
        /// Gets the contractor identifier.
        /// </summary>
        int ContractorID { get; }

        /// <summary>
        /// Gets the contract number.
        /// </summary>
        string ContractNumber { get; }

        /// <summary>
        /// Gets the major version.
        /// </summary>
        int MajorVersion { get; }

        /// <summary>
        /// Gets the minor version.
        /// </summary>
        int MinorVersion { get; }

        /// <summary>
        /// Gets the start date.
        /// </summary>
        DateTime? StartDate { get; }

        /// <summary>
        /// Gets the end date.
        /// </summary>
        DateTime? EndDate { get; }
    }
}
