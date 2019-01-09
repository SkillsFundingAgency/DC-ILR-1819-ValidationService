using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the FCS contract allocation definition
    /// </summary>
    public interface IFcsContractAllocation :
        IIdentifiableItem,
        IEsfEligibilityRuleReferences
    {
        /// <summary>
        /// Gets the contract identifier.
        /// </summary>
        int ContractID { get; }

        /// <summary>
        /// Gets the contract allocation number.
        /// </summary>
        string ContractAllocationNumber { get; }

        /// <summary>
        /// Gets the funding stream code.
        /// </summary>
        string FundingStreamCode { get; }

        /// <summary>
        /// Gets the funding stream period code.
        /// </summary>
        string FundingStreamPeriodCode { get; }

        /// <summary>
        /// Gets the period.
        /// </summary>
        string Period { get; }

        /// <summary>
        /// Gets the period type code.
        /// </summary>
        string PeriodTypeCode { get; }

        /// <summary>
        /// Gets the start date.
        /// </summary>
        DateTime? StartDate { get; }

        /// <summary>
        /// Gets the end date.
        /// </summary>
        DateTime? EndDate { get; }

        /// <summary>
        /// Gets the stop new starts from date.
        /// </summary>
        DateTime? StopNewStartsFromDate { get; }

        /// <summary>
        /// Gets the termination date.
        /// </summary>
        DateTime? TerminationDate { get; }

        /// <summary>
        /// Gets the delivery UKPRN.
        /// </summary>
        int DeliveryUKPRN { get; }

        /// <summary>
        /// Gets the delivery organisation.
        /// </summary>
        string DeliveryOrganisation { get; }

        /// <summary>
        /// Gets the learning rate premium factor.
        /// </summary>
        decimal? LearningRatePremiumFactor { get; }

        /// <summary>
        /// Gets the UOP code.
        /// </summary>
        string UoPCode { get; }
    }
}
