using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    /// <summary>
    /// a test class to test out the complexities of the learn aim ref rule
    /// </summary>
    /// <seealso cref="LearnAimRefRuleBase" />
    public class LearnAimRefRuleBaseTestRule :
        LearnAimRefRuleBase
    {
        /// <summary>
        /// is set to pass, in this test version
        /// </summary>
        private bool _isSetToPass;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnAimRefRuleBaseTestRule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="larsData">The lars data.</param>
        public LearnAimRefRuleBaseTestRule(
            IValidationErrorHandler validationErrorHandler,
            IProvideLearnAimRefRuleActions provider,
            ILARSDataService larsData)
                : base(validationErrorHandler, provider, larsData, "LearnAimRefRuleBaseTestRule")
        {
        }

        /// <summary>
        /// Sets the pass value.
        /// </summary>
        /// <param name="setToPass">if set to <c>true</c> [set to pass].</param>
        public void SetPassValue(bool setToPass)
        {
            _isSetToPass = setToPass;
        }

        /// <summary>
        /// Passes the (rule) conditions.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="branch">The branch.</param>
        /// <returns>
        /// true if it does...
        /// </returns>
        public override bool PassesConditions(ILearningDelivery delivery, IBranchResult branch)
        {
            return branch.OutOfScope || _isSetToPass;
        }
    }
}
