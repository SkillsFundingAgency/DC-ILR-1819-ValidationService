using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Model
{
    /// <summary>
    /// the FCS Contract Allocation implementation
    /// </summary>
    /// <seealso cref="IFcsContractAllocation" />
    public class FcsContractAllocation :
        IFcsContractAllocation
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the contract identifier.
        /// </summary>
        public int ContractID { get; set; }

        /// <summary>
        /// Gets or sets the contract allocation number.
        /// </summary>
        public string ContractAllocationNumber { get; set; }

        /// <summary>
        /// Gets or sets the tender spec reference.
        /// </summary>
        public string TenderSpecReference { get; set; }

        /// <summary>
        /// Gets or sets the lot reference.
        /// </summary>
        public string LotReference { get; set; }

        /// <summary>
        /// Gets or sets the funding stream code.
        /// </summary>
        public string FundingStreamCode { get; set; }

        /// <summary>
        /// Gets or sets the funding stream period code.
        /// </summary>
        public string FundingStreamPeriodCode { get; set; }

        /// <summary>
        /// Gets or sets the period.
        /// </summary>
        public string Period { get; set; }

        /// <summary>
        /// Gets or sets the period type code.
        /// </summary>
        public string PeriodTypeCode { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the stop new starts from date.
        /// </summary>
        public DateTime? StopNewStartsFromDate { get; set; }

        /// <summary>
        /// Gets or sets the termination date.
        /// </summary>
        public DateTime? TerminationDate { get; set; }

        /// <summary>
        /// Gets or sets the delivery UKPRN.
        /// </summary>
        public int DeliveryUKPRN { get; set; }

        /// <summary>
        /// Gets or sets the delivery organisation.
        /// </summary>
        public string DeliveryOrganisation { get; set; }

        /// <summary>
        /// Gets or sets the learning rate premium factor.
        /// </summary>
        public decimal? LearningRatePremiumFactor { get; set; }

        /// <summary>
        /// Gets or sets the UOP code.
        /// </summary>
        public string UoPCode { get; set; }
    }
}
