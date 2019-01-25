using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    /// <summary>
    /// learn aim ref rule 88
    /// </summary>
    /// <seealso cref="LearnAimRefRuleBase" />
    public class LearnAimRef_88Rule :
        LearnAimRefRuleBase
    {
        /// <summary>
        /// The (rule) name
        /// </summary>
        public const string Name = "LearnAimRef_88";

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnAimRef_88Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="larsData">The lars data.</param>
        /// <param name="derivedData07">The derived data 07 (rule).</param>
        /// <param name="derivedData11">The derived data 11 (rule).</param>
        public LearnAimRef_88Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IDerivedData_07Rule derivedData07,
            IDerivedData_11Rule derivedData11)
                : base(validationErrorHandler, larsData, derivedData07, derivedData11)
        {
        }

        /// <summary>
        /// Determines whether [has valid start range] [the specified validity].
        /// caters for the custom and practice of setting the end date to before
        /// the start date as a means of withdrawing funding
        /// </summary>
        /// <param name="validity">The validity.</param>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [in valid start range] [the specified validity]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasValidStartRange(ILARSLearningDeliveryValidity validity, ILearningDelivery delivery) =>
            validity.IsCurrent(delivery.LearnStartDate, validity.LastNewStartDate);

        /// <summary>
        /// Determines whether [has valid learning aim] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="branchCategory">The branch category.</param>
        /// <returns>
        ///   <c>true</c> if [has valid learning aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasValidLearningAim(ILearningDelivery delivery, string branchCategory)
        {
            var validities = LarsData.GetValiditiesFor(delivery.LearnAimRef)
                .Where(x => x.ValidityCategory.ComparesWith(branchCategory));

            return validities
                .SafeAny(x => HasValidStartRange(x, delivery));
        }

        /// <summary>
        /// Gets the (rule) name.
        /// </summary>
        /// <returns>
        /// the rule name
        /// </returns>
        public override string GetName()
        {
            return Name;
        }

        /// <summary>
        /// Passes the (rule) conditions.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="branch">The branch result.</param>
        /// <returns>
        /// true if it does...
        /// </returns>
        public override bool PassesConditions(ILearningDelivery delivery, BranchResult branch)
        {
            return branch.OutOfScope || HasValidLearningAim(delivery, branch.Category);
        }
    }
}
