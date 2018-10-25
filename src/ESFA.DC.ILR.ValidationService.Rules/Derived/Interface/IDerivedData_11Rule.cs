using ESFA.DC.ILR.Model.Interface;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    /// <summary>
    /// derived data rule 21
    /// Adult skills funded unemployed learner on other state benefits on learning aim start date
    /// </summary>
    public interface IDerivedData_11Rule
    {
        /// <summary>
        /// Determines whether [is adult funded on benefits at start of aim] [the specified candidate].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learnerEmployments">The learner employments.</param>
        /// <returns>
        ///   <c>true</c> if [is adult funded on benefits at start of aim] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        bool IsAdultFundedOnBenefitsAtStartOfAim(ILearningDelivery delivery, IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmployments);
    }
}
