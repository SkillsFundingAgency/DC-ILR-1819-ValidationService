using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_14Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The derived data 22 (rule)
        /// </summary>
        private readonly IDerivedData_22Rule _derivedData22;

        /// <summary>
        /// The (academic) year data (service)
        /// </summary>
        private readonly IFCSDataService _fcsData;

        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpStat_14Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="derivedData22">The derived data 22 rule.</param>
        /// <param name="fcsData">The year data.</param>
        /// <param name="commonOperations">The common operations.</param>
        public EmpStat_14Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_22Rule derivedData22,
            IFCSDataService fcsData,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.EmpStat_14)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(derivedData22)
                .AsGuard<ArgumentNullException>(nameof(derivedData22));
            It.IsNull(fcsData)
                .AsGuard<ArgumentNullException>(nameof(fcsData));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _derivedData22 = derivedData22;
            _fcsData = fcsData;
            _check = commonOperations;
        }

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
        /// Gets the qualifying aim on this date using sources.
        /// the incoming set is guaranteed not to be null
        /// </summary>
        /// <param name="thisDate">this date.</param>
        /// <param name="usingSources">using sources.</param>
        /// <returns>
        /// the qualifying aim on this date
        /// </returns>
        public ILearningDelivery GetQualifyingdAimOn(DateTime? thisDate, IReadOnlyCollection<ILearningDelivery> usingSources) =>
            usingSources
                .FirstOrDefault(x => x.LearnStartDate == thisDate
                    && x.FundModel == TypeOfFunding.EuropeanSocialFund
                    && x.LearnAimRef == TypeOfAim.References.ESFLearnerStartandAssessment
                    && x.CompStatus == CompletionState.HasCompleted);

        /// <summary>
        /// Gets the eligible employment status.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>the eligible employment status for the allocated contract</returns>
        public IReadOnlyCollection<IEsfEligibilityRuleEmploymentStatus> GetEligibilityRulesFor(ILearningDelivery delivery) =>
            _fcsData.GetEligibilityRuleEmploymentStatusesFor(delivery?.ConRefNumber).AsSafeReadOnlyList();

        /// <summary>
        /// Determines whether [has a qualifying employment status] [the specified employment].
        /// at the point of calling, neither the employment or the eligibility can be null
        /// </summary>
        /// <param name="eligibility">The eligibility (status).</param>
        /// <param name="thisEmployment">this employment.</param>
        /// <returns>
        ///   <c>true</c> if [has a qualifying employment status] [the specified this employment]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasAQualifyingEmploymentStatus(IEsfEligibilityRuleEmploymentStatus eligibility, ILearnerEmploymentStatus thisEmployment) =>
            eligibility.Code == thisEmployment.EmpStat;

        public bool IsNotValid(IEsfEligibilityRuleEmploymentStatus eligibility, ILearnerEmploymentStatus employment) =>
            !HasAQualifyingEmploymentStatus(eligibility, employment);

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
            var qualifyingDate = GetLatestContractCompletionDate(fromDeliveries);
            var qualifyingAim = GetQualifyingdAimOn(qualifyingDate, fromDeliveries);
            var eligibilities = GetEligibilityRulesFor(qualifyingAim);

            if (It.IsEmpty(eligibilities))
            {
                return;
            }

            var fromEmployments = objectToValidate.LearnerEmploymentStatuses.AsSafeReadOnlyList();
            var employment = _check.GetEmploymentStatusOn(qualifyingDate, fromEmployments);

            if (It.IsNull(employment))
            {
                return;
            }

            eligibilities
                .ForAny(x => IsNotValid(x, employment), x => RaiseValidationMessage(learnRefNumber, qualifyingAim, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="thisEligibility">this eligibility.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery, IEsfEligibilityRuleEmploymentStatus thisEligibility)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery, thisEligibility));
        }

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="thisEligibility">this eligibility.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery, IEsfEligibilityRuleEmploymentStatus thisEligibility)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.EmpStat, thisEligibility.Code),
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, thisDelivery.ConRefNumber),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate)
            };
        }
    }
}
