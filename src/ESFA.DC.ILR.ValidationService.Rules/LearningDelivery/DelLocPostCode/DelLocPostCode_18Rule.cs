using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.DelLocPostCode
{
    /// <summary>
    /// the delivery location postcode rule 18 implementation
    /// </summary>
    /// <seealso cref="DelLocPostCode.DelLocPostCode_EligibilityRuleBase{IEsfEligibilityRuleLocalEnterprisePartnership}" />
    public class DelLocPostCode_18Rule :
        DelLocPostCode_EligibilityRuleBase<IEsfEligibilityRuleLocalEnterprisePartnership>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelLocPostCode_18Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonChecks">The common checks.</param>
        /// <param name="fcsData">The FCS data.</param>
        /// <param name="postcodesData">The postcodes data.</param>
        /// <param name="derivedData22">The derived data22.</param>
        public DelLocPostCode_18Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonChecks,
            IFCSDataService fcsData,
            IPostcodesDataService postcodesData,
            IDerivedData_22Rule derivedData22)
            : base(
                validationErrorHandler,
                commonChecks,
                fcsData,
                postcodesData,
                derivedData22)
        {
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        /// <returns>
        /// the name of the rule
        /// </returns>
        public override string GetName() => "DelLocPostCode_18";

        /// <summary>
        /// Gets the eligibility item.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>the enterprise partnership (if found)</returns>
        public override IEsfEligibilityRuleLocalEnterprisePartnership GetEligibilityItem(ILearningDelivery delivery) =>

            // This is wrong, it should return collection
            FcsData.GetEligibilityRuleEnterprisePartnershipsFor(delivery.ConRefNumber)?.FirstOrDefault();

        /// <summary>
        /// Determines whether [has qualifying eligibility] [the specified postcode].
        /// </summary>
        /// <param name="postcode">The postcode.</param>
        /// <param name="eligibility">The eligibility.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying eligibility] [the specified postcode]; otherwise, <c>false</c>.
        /// </returns>
        public override bool HasQualifyingEligibility(IONSPostcode postcode, IEsfEligibilityRuleLocalEnterprisePartnership eligibility) =>
            It.Has(postcode)
                && It.Has(eligibility)
                && eligibility.Code.ComparesWith(postcode.Lep1, postcode.Lep2);
    }
}
