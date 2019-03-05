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
    /// <summary>
    /// the delivery location postcode rule 18 implementation
    /// </summary>
    /// <seealso cref="DelLocPostCode.DelLocPostCode_EligibilityRule{IEsfEligibilityRuleLocalEnterprisePartnership}" />
    public class DelLocPostCode_18Rule :
        DelLocPostCode_EligibilityRule<IEsfEligibilityRuleLocalEnterprisePartnership>
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
            : base(validationErrorHandler, commonChecks, fcsData, postcodesData, derivedData22, RuleNameConstants.DelLocPostCode_18)
        {
        }

        /// <summary>
        /// Gets the eligibility items for...
        /// </summary>
        /// <param name="delivery">This delivery.</param>
        /// <returns>the enterprise partnership (if found)</returns>
        public override IReadOnlyCollection<IEsfEligibilityRuleLocalEnterprisePartnership> GetEligibilityItemsFor(ILearningDelivery delivery) =>
            FcsData.GetEligibilityRuleEnterprisePartnershipsFor(delivery?.ConRefNumber).AsSafeReadOnlyList();

        /// <summary>
        /// Determines whether [has qualifying eligibility] [the specified postcode].
        /// </summary>
        /// <param name="delivery">The latest learnstartdate delivery.</param>
        /// <param name="postcodes">The postcode.</param>
        /// <param name="eligibilities">The eligibilities.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying eligibility] [the specified postcode]; otherwise, <c>false</c>.
        /// </returns>
        public override bool HasQualifyingEligibility(ILearningDelivery delivery, IReadOnlyCollection<IONSPostcode> postcodes, IReadOnlyCollection<IEsfEligibilityRuleLocalEnterprisePartnership> eligibilities) =>
            It.Has(postcodes)
            && It.Has(eligibilities)
            && It.Has(delivery)
            && eligibilities.Any(eli => postcodes.Any(pc => (pc.Lep1.ComparesWith(eli.Code) || pc.Lep2.ComparesWith(eli.Code)) && InQualifyingPeriod(delivery, pc)));
    }
}
