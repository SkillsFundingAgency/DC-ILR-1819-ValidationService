using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;

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
        /// Initializes a new instance of the <see cref="LearnAimRefRuleBaseTestRule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="larsData">The lars data.</param>
        /// <param name="derivedData07">The derived data 07 (rule).</param>
        /// <param name="derivedData11">The derived data 11 (rule).</param>
        public LearnAimRefRuleBaseTestRule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IDerivedData_07Rule derivedData07,
            IDerivedData_11Rule derivedData11)
                : base(validationErrorHandler, larsData, derivedData07, derivedData11)
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
        /// Gets the (rule) name.
        /// </summary>
        /// <returns>
        /// the rule name
        /// </returns>
        public override string GetName()
        {
            return "LearnAimRefRuleBaseTestRule";
        }

        /// <summary>
        /// Passes the (rule) conditions.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="branch">The branch.</param>
        /// <returns>
        /// true if it does...
        /// </returns>
        public override bool PassesConditions(ILearningDelivery delivery, BranchResult branch)
        {
            return branch.OutOfScope || _isSetToPass;
        }
    }
}
