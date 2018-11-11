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
        /// Gets or sets the description.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets the deliverable code.
        /// </summary>
        int? DeliverableCode { get; set; }

        /// <summary>
        /// Gets or sets the unit cost.
        /// </summary>
        decimal? UnitCost { get; set; }

        /// <summary>
        /// Gets or sets the planned volume.
        /// </summary>
        int? PlannedVolume { get; set; }

        /// <summary>
        /// Gets or sets the planned value.
        /// </summary>
        decimal? PlannedValue { get; set; }
    }
}
