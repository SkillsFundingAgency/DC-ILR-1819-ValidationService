using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.DelLocPostCode
{
    public abstract class DelLocPostCode_EligibilityRule<TEligibilityItem> :
        AbstractRule,
        IRule<ILearner>
        where TEligibilityItem : class, IEsfEligibilityRuleReferences
    {
        /// <summary>
        /// The common rule (operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// The postcodes data (service)
        /// </summary>
        private readonly IPostcodesDataService _postcodesData;

        /// <summary>
        /// The derived data 22 (rule)
        /// </summary>
        private readonly IDerivedData_22Rule _derivedData22;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelLocPostCode_EligibilityRule{TEligibilityItem}" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonChecks">The common checks.</param>
        /// <param name="fcsData">The FCS data.</param>
        /// <param name="postcodesData">The postcodes data.</param>
        /// <param name="derivedData22">The derived data22.</param>
        /// <param name="ruleName">Name of the rule.</param>
        public DelLocPostCode_EligibilityRule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonChecks,
            IFCSDataService fcsData,
            IPostcodesDataService postcodesData,
            IDerivedData_22Rule derivedData22,
            string ruleName)
            : base(validationErrorHandler, ruleName)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonChecks)
                .AsGuard<ArgumentNullException>(nameof(commonChecks));
            It.IsNull(fcsData)
                .AsGuard<ArgumentNullException>(nameof(fcsData));
            It.IsNull(postcodesData)
                .AsGuard<ArgumentNullException>(nameof(postcodesData));
            It.IsNull(derivedData22)
                .AsGuard<ArgumentNullException>(nameof(derivedData22));

            _check = commonChecks;
            FcsData = fcsData;
            _postcodesData = postcodesData;
            _derivedData22 = derivedData22;
        }

        /// <summary>
        /// Gets the first viable date.
        /// </summary>
        public static DateTime FirstViableDate => new DateTime(2017, 08, 01);

        /// <summary>
        /// Gets the FCS data (service)
        /// </summary>
        protected IFCSDataService FcsData { get; }

        /// <summary>
        /// Gets the contract completion date.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="sources">The sources.</param>
        /// <returns>the latest date for any specific contract reference</returns>
        public DateTime? GetContractCompletionDate(
            ILearningDelivery delivery,
            IReadOnlyCollection<ILearningDelivery> sources) =>
                _derivedData22.GetLatestLearningStartForESFContract(delivery, sources);

        /// <summary>
        /// Gets the latest start for completed contract.
        /// </summary>
        /// <param name="usingSources">using sources.</param>
        /// <returns>the latest start across all the completed contracts (if there is one)</returns>
        public DateTime? GetLatestStartForCompletedContract(IReadOnlyCollection<ILearningDelivery> usingSources)
        {
            var candidates = Collection.Empty<DateTime?>();
            usingSources.ForEach(source => candidates.Add(GetContractCompletionDate(source, usingSources)));

            return candidates.Max();
        }

        /// <summary>
        /// Gets the (esf eligibility rule) enterprise partnership.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>the enterprise partnership (if found)</returns>
        public abstract IReadOnlyCollection<TEligibilityItem> GetEligibilityItemsFor(ILearningDelivery delivery);

        /// <summary>
        /// Gets the ons postcode.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>the ons postcode (if found)</returns>
        //public IONSPostcode GetONSPostcode(ILearningDelivery delivery) =>
        //    _postcodesData.GetONSPostcode(delivery.DelLocPostCode);

        /// <summary>
        /// Gets the ons postcode.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>the ons postcodes (if found)</returns>
        public IReadOnlyCollection<IONSPostcode> GetONSPostcodes(ILearningDelivery delivery) =>
            _postcodesData.GetONSPostcodes(delivery.DelLocPostCode);

        /// <summary>
        /// Determines whether [has qualifying eligibility] [the specified postcode].
        /// </summary>
        /// <param name="delivery">The latest learnstartdate delivery.</param>
        /// <param name="postcode">The postcode.</param>
        /// <param name="eligibilities">The eligibilities.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying eligibility] [the specified postcode]; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool HasQualifyingEligibility(ILearningDelivery delivery, IReadOnlyCollection<IONSPostcode> postcode, IReadOnlyCollection<TEligibilityItem> eligibilities);

        /// <summary>
        /// Determines whether [in qualifying period] [the specified delivery].
        /// TODO: needs to cater for 'termination' but this is currently a string of indeterminate quality
        /// It.IsBetween(delivery.LearnStartDate, onsPostcode.EffectiveFrom, onsPostcode.EffectiveTo ?? onsPostcode.Termination ?? DateTime.MaxValue);
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="onsPostcode">The ons postcode.</param>
        /// <returns>
        ///   <c>true</c> if [in qualifying period] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool InQualifyingPeriod(ILearningDelivery delivery, IONSPostcode onsPostcode) =>
            delivery.LearnStartDate < onsPostcode.EffectiveFrom
            || delivery.LearnStartDate > (onsPostcode.EffectiveTo ?? DateTime.MaxValue)
            || delivery.LearnStartDate >= (onsPostcode.Termination ?? DateTime.MaxValue);

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery delivery) =>
            _check.HasQualifyingStart(delivery, FirstViableDate)
                && _check.HasQualifyingFunding(delivery, TypeOfFunding.EuropeanSocialFund)
                && HasQualifyingEligibility(delivery, GetONSPostcodes(delivery), GetEligibilityItemsFor(delivery));

        /// <summary>
        /// Matches start.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learnStart">The learn start.</param>
        /// <returns>true if it does</returns>
        public bool MatchesStart(ILearningDelivery delivery, DateTime learnStart) =>
            delivery.LearnStartDate == learnStart;

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            var contractStart = GetLatestStartForCompletedContract(objectToValidate.LearningDeliveries);
            if (It.IsEmpty(contractStart))
            {
                return;
            }

            objectToValidate.LearningDeliveries
                .SafeWhere(x => MatchesStart(x, contractStart.Value))
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
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
        /// Builds the message parameters for..
        /// </summary>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, thisDelivery.LearnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, thisDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.DelLocPostCode, thisDelivery.DelLocPostCode),
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, thisDelivery.ConRefNumber)
            };
        }
    }
}
