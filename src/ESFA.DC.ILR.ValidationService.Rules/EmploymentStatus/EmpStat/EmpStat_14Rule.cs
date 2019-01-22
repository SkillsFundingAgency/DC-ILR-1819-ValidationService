using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_14Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = PropertyNameConstants.EmpStat;

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "EmpStat_14";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The derived data 22 (rule)
        /// </summary>
        private readonly IDerivedData_22Rule _derivedData22;

        /// <summary>
        /// The (academic) year data (service)
        /// </summary>
        private readonly IFCSDataService _fcsData;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpStat_14Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="derivedData22">The derived data 22 rule.</param>
        /// <param name="fcsData">The year data.</param>
        public EmpStat_14Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_22Rule derivedData22,
            IFCSDataService fcsData)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(derivedData22)
                .AsGuard<ArgumentNullException>(nameof(derivedData22));
            It.IsNull(fcsData)
                .AsGuard<ArgumentNullException>(nameof(fcsData));

            _messageHandler = validationErrorHandler;
            _derivedData22 = derivedData22;
            _fcsData = fcsData;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Gets the contract completion date.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="usingSources">The using sources.</param>
        /// <returns>the latest completion date for the contract (if there is one)</returns>
        public DateTime? GetContractCompletionDate(ILearningDelivery delivery, IReadOnlyCollection<ILearningDelivery> usingSources) =>
            _derivedData22.GetLatestLearningStartForESFContract(delivery, usingSources);

        /// <summary>
        /// Gets the latest contract completion date.
        /// </summary>
        /// <param name="usingSources">The using sources.</param>
        /// <returns>the latest completion date for all the contracts (if there is one)</returns>
        public DateTime? GetLatestContractCompletionDate(IReadOnlyCollection<ILearningDelivery> usingSources)
        {
            var candidates = Collection.Empty<DateTime?>();
            usingSources.ForEach(source => candidates.Add(GetContractCompletionDate(source, usingSources)));

            return candidates.Max();
        }

        /// <summary>
        /// Gets the qualifyingd aim.
        /// the incoming set is guaranteed not to be null
        /// </summary>
        /// <param name="usingSources">using sources.</param>
        /// <param name="onThisDate">on this date.</param>
        /// <returns>the qualifying aim on this date</returns>
        public ILearningDelivery GetQualifyingdAim(IReadOnlyCollection<ILearningDelivery> usingSources, DateTime? onThisDate) =>
            usingSources
                .FirstOrDefault(x => x.LearnStartDate == onThisDate
                    && x.FundModel == TypeOfFunding.EuropeanSocialFund
                    && x.LearnAimRef == TypeOfAim.References.ESFLearnerStartandAssessment
                    && x.CompStatus == CompletionState.HasCompleted);

        /// <summary>
        /// Gets the eligible employment status.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>the eligible employment status for the allocated contract</returns>
        public IEsfEligibilityRuleEmploymentStatus GetEligibleEmploymentStatus(ILearningDelivery delivery) =>

            // This is wrong, it should return collection
            _fcsData.GetEligibilityRuleEmploymentStatusesFor(delivery?.ConRefNumber)?.FirstOrDefault();

        /// <summary>
        /// Gets the closest employment.
        /// the incoming set is guaranteed not to be null
        /// </summary>
        /// <param name="usingSources">The using sources.</param>
        /// <param name="toThisDate">To this date.</param>
        /// <returns>the closest employment record to this date (if there is one)</returns>
        public ILearnerEmploymentStatus GetClosestEmployment(IReadOnlyCollection<ILearnerEmploymentStatus> usingSources, DateTime? toThisDate) =>
            usingSources
                .Where(x => x.DateEmpStatApp <= toThisDate)
                .OrderByDescending(x => x.DateEmpStatApp)
                .FirstOrDefault();

        /// <summary>
        /// Determines whether [has a qualifying employment status] [the specified employment].
        /// at the point of calling, neither the employment or the eligibility can be null
        /// </summary>
        /// <param name="thisEmployment">this employment.</param>
        /// <param name="eligibility">The eligibility (status).</param>
        /// <returns>
        ///   <c>true</c> if [has a qualifying employment status] [the specified this employment]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasAQualifyingEmploymentStatus(ILearnerEmploymentStatus thisEmployment, IEsfEligibilityRuleEmploymentStatus eligibility) =>
            thisEmployment.EmpStat == eligibility.Code;

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            var fromDeliveries = objectToValidate.LearningDeliveries.AsSafeReadOnlyList();
            var fromEmployments = objectToValidate.LearnerEmploymentStatuses.AsSafeReadOnlyList();

            var qualifyingDate = GetLatestContractCompletionDate(fromDeliveries);
            var qualifyingAim = GetQualifyingdAim(fromDeliveries, qualifyingDate);
            var eligibleStatus = GetEligibleEmploymentStatus(qualifyingAim);

            if (It.IsNull(eligibleStatus))
            {
                return;
            }

            var employment = GetClosestEmployment(fromEmployments, qualifyingDate);

            if (It.IsNull(employment))
            {
                return;
            }

            if (!HasAQualifyingEmploymentStatus(employment, eligibleStatus))
            {
                RaiseValidationMessage(learnRefNumber, qualifyingAim, employment);
            }
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="thisEmployment">this employment.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery, ILearnerEmploymentStatus thisEmployment)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, thisEmployment.EmpStat));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, thisDelivery.ConRefNumber));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate));

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
