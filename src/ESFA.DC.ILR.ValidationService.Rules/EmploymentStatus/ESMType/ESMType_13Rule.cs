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

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType
{
    public class ESMType_13Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The checker (common rule operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// The fcs data (service)
        /// </summary>
        private readonly IFCSDataService _fcsData;

        /// <summary>
        /// derived data rule 25
        /// </summary>
        private readonly IDerivedData_25Rule _ddrule25;

        /// <summary>
        /// Initializes a new instance of the <see cref="ESMType_13Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="ddrule25">derived data rule 25.</param>
        /// <param name="fcsData">The lars data.</param>
        /// <param name="commonOperations">The common operations provider.</param>
        public ESMType_13Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_25Rule ddrule25,
            IFCSDataService fcsData,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.ESMType_13)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(ddrule25)
                .AsGuard<ArgumentNullException>(nameof(ddrule25));
            It.IsNull(fcsData)
                .AsGuard<ArgumentNullException>(nameof(fcsData));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _ddrule25 = ddrule25;
            _fcsData = fcsData;
            _check = commonOperations;
        }

        /// <summary>
        /// Gets the eligibility rule for (this delivery).
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>an eligibility rule</returns>
        public IEsfEligibilityRule GetEligibilityRuleFor(ILearningDelivery thisDelivery) =>
            _fcsData.GetEligibilityRuleFor(thisDelivery.ConRefNumber);

        /// <summary>
        /// Gets the derived rules length of unemployment indicator for.
        /// </summary>
        /// <param name="thisLearner">this learner.</param>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>the length of unemployment</returns>
        public int? GetDerivedRuleLOUIndicatorFor(ILearner thisLearner, ILearningDelivery thisDelivery) =>
            _ddrule25.GetLengthOfUnemployment(thisLearner, thisDelivery.ConRefNumber);

        /// <summary>
        /// Determines whether [has disqualifying minimum lou indicator] [the specified eligibility].
        /// </summary>
        /// <param name="eligibility">The eligibility.</param>
        /// <param name="derivedRuleResult">The derived rule result.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying minimum lou indicator] [the specified eligibility]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingMinLOUIndicator(IEsfEligibilityRule eligibility, int derivedRuleResult) =>
            It.Has(eligibility.MinLengthOfUnemployment)
            && derivedRuleResult < eligibility.MinLengthOfUnemployment;

        /// <summary>
        /// Determines whether [has disqualifying maximum lou indicator] [the specified eligibility].
        /// </summary>
        /// <param name="eligibility">The eligibility.</param>
        /// <param name="derivedRuleResult">The derived rule result.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying maximum lou indicator] [the specified eligibility]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingMaxLOUIndicator(IEsfEligibilityRule eligibility, int derivedRuleResult) =>
            It.Has(eligibility.MaxLengthOfUnemployment)
            && derivedRuleResult > eligibility.MaxLengthOfUnemployment;

        /// <summary>
        /// Determines whether [has disqualifying lou indicator] [the specified eligibility].
        /// </summary>
        /// <param name="eligibility">The eligibility.</param>
        /// <param name="derivedRuleResult">The derived rule result.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying lou indicator] [the specified eligibility]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingLOUIndicator(IEsfEligibilityRule eligibility, int? derivedRuleResult) =>
            It.Has(derivedRuleResult)
            && (HasDisqualifyingMinLOUIndicator(eligibility, derivedRuleResult.Value)
            || HasDisqualifyingMaxLOUIndicator(eligibility, derivedRuleResult.Value));

        /// <summary>
        /// Determines whether [has disqualifying lou indicator] [the specified this delivery].
        /// </summary>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <param name="derivedRuleAction">The derived rule action.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying lou indicator] [the specified this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingLOUIndicator(ILearningDelivery thisDelivery, Func<int?> derivedRuleAction) =>
            HasDisqualifyingLOUIndicator(GetEligibilityRuleFor(thisDelivery), derivedRuleAction());

        /// <summary>
        /// Determines whether [is not valid] [this delivery].
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="derivedRuleAction">The derived rule action.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery thisDelivery, Func<int?> derivedRuleAction) =>
            _check.HasQualifyingFunding(thisDelivery, TypeOfFunding.EuropeanSocialFund)
                && HasDisqualifyingLOUIndicator(thisDelivery, derivedRuleAction);

        /// <summary>
        /// Validates this learner.
        /// </summary>
        /// <param name="thisLearner">this learner.</param>
        public void Validate(ILearner thisLearner)
        {
            It.IsNull(thisLearner)
                .AsGuard<ArgumentNullException>(nameof(thisLearner));

            var learnRefNumber = thisLearner.LearnRefNumber;

            thisLearner.LearningDeliveries
                .ForAny(x => IsNotValid(x, () => GetDerivedRuleLOUIndicatorFor(thisLearner, x)), x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, null, BuildMessageParametersFor(thisDelivery));
        }

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, thisDelivery.ConRefNumber)
            };
        }
    }
}