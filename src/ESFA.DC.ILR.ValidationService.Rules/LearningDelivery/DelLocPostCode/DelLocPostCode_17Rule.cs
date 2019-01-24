using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.DelLocPostCode
{
    public class DelLocPostCode_17Rule :
        DelLocPostCode_EligibilityRule<IEsfEligibilityRuleLocalAuthority>
    {
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
            : base(validationErrorHandler, commonChecks, fcsData, postcodesData, derivedData22, RuleNameConstants.DelLocPostCode_17)
        {
        }

        /// <summary>
        /// Gets the eligibility item.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>the enterprise partnership (if found)</returns>
        public override IReadOnlyCollection<IEsfEligibilityRuleLocalAuthority> GetEligibilityItem(ILearningDelivery delivery) =>
            FcsData.GetEligibilityRuleLocalAuthoritiesFor(delivery.ConRefNumber).AsSafeReadOnlyList();

        /// <summary>
        /// Determines whether [has qualifying eligibility] [the specified postcode].
        /// </summary>
        /// <param name="postcode">The postcode.</param>
        /// <param name="eligibilities">The eligibilities.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying eligibility] [the specified postcode]; otherwise, <c>false</c>.
        /// </returns>
        public override bool HasQualifyingEligibility(IONSPostcode postcode, IReadOnlyCollection<IEsfEligibilityRuleLocalAuthority> eligibilities) =>
            It.Has(postcode)
                && eligibilities.SafeAny(x => x.Code.ComparesWith(postcode.LocalAuthority));
    }
}
