using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    /// <summary>
    /// derived data rule 28
    /// Adult skills funded learner unemployed with benefits on learning aim start date
    /// </summary>
    public interface IDerivedData_28Rule
    {
        /// <summary>
        /// Determines whether [is adult skills unemployed with benefits] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is adult skills unemployed with benefits] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        bool IsAdultFundedUnemployedWithBenefits(ILearner candidate);
    }
}
