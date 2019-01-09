namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the FCS contract delviverable definition
    /// </summary>
    public interface IFcsContractDeliverable :
        IIdentifiableItem
    {
        /// <summary>
        /// Gets the contract allocation identifier.
        /// </summary>
        int ContractAllocationID { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the deliverable code.
        /// </summary>
        int? DeliverableCode { get; }

        /// <summary>
        /// Gets the unit cost.
        /// </summary>
        decimal? UnitCost { get; }

        /// <summary>
        /// Gets the planned volume.
        /// </summary>
        int? PlannedVolume { get; }

        /// <summary>
        /// Gets the planned value.
        /// </summary>
        decimal? PlannedValue { get; }
    }
}
