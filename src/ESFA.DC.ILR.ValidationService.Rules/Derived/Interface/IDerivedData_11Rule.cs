﻿using ESFA.DC.ILR.Model.Interface;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    /// <summary>
    /// derived data rule 11
    /// Adult skills funded learner on benefits at the start of the learning aim
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
