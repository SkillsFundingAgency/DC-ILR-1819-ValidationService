using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.DelLocPostCode
{
    public class DelLocPostCode_17Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = "DelLocPostCode_17";

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The common rule (operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// The FCS data (service)
        /// </summary>
        private readonly IFCSDataService _fcsData;

        /// <summary>
        /// The postcodes data (service)
        /// </summary>
        private readonly IPostcodesDataService _postcodesData;

        /// <summary>
        /// The derived data 22 (rule)
        /// </summary>
        private readonly IDerivedData_22Rule _derivedData22;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelLocPostCode_17Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonChecks">The common checks.</param>
        /// <param name="fcsData">The FCS data.</param>
        /// <param name="postcodesData">The postcodes data.</param>
        /// <param name="derivedData22">The derived data22.</param>
        public DelLocPostCode_17Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonChecks,
            IFCSDataService fcsData,
            IPostcodesDataService postcodesData,
            IDerivedData_22Rule derivedData22)
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

            _messageHandler = validationErrorHandler;
            _check = commonChecks;
            _fcsData = fcsData;
            _postcodesData = postcodesData;
            _derivedData22 = derivedData22;
        }

        /// <summary>
        /// Gets the first viable date.
        /// </summary>
        public static DateTime FirstViableDate => new DateTime(2017, 08, 01);

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

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
        /// Gets the (esf eligibility rule) local authority.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>the authority (if found)</returns>
        public IEsfEligibilityRuleLocalAuthority GetLocalAuthority(ILearningDelivery delivery) =>
            _fcsData.GetEligibilityRuleLocalAuthorityFor(delivery.ConRefNumber);

        /// <summary>
        /// Gets the ons postcode.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>the ons postcode (if found)</returns>
        public IONSPostcode GetONSPostcode(ILearningDelivery delivery) =>
            _postcodesData.GetONSPostcode(delivery.DelLocPostCode);

        /// <summary>
        /// Determines whether [is matching code] [the specified delivery].
        /// </summary>
        /// <param name="postcode">The postcode.</param>
        /// <param name="authority">The authority.</param>
        /// <returns>
        ///   <c>true</c> if [is matching code] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingLocalAuthority(IONSPostcode postcode, IEsfEligibilityRuleLocalAuthority authority) =>
            It.Has(postcode)
                && It.Has(authority)
                && authority.Code.ComparesWith(postcode.LocalAuthority);

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
            It.IsBetween(delivery.LearnStartDate, onsPostcode.EffectiveFrom, onsPostcode.EffectiveTo ?? DateTime.MaxValue);

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
                && HasQualifyingLocalAuthority(GetONSPostcode(delivery), GetLocalAuthority(delivery))
                && !InQualifyingPeriod(delivery, GetONSPostcode(delivery));

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

            var deliveries = objectToValidate.LearningDeliveries
                .SafeWhere(x => MatchesStart(x, contractStart.Value))
                .AsSafeReadOnlyList();

            deliveries
                .Where(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.LearnAimRef), thisDelivery.LearnAimRef));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.FundModel), thisDelivery.FundModel));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.DelLocPostCode), thisDelivery.DelLocPostCode));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisDelivery.ConRefNumber), thisDelivery.ConRefNumber));

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
