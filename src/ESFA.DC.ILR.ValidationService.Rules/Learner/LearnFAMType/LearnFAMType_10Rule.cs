using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType
{
    /// <summary>
    /// If the FAM type is LSR there must be no more than four occurrences
    /// </summary>
    public class LearnFAMType_10Rule : AbstractRule, IRule<ILearner>
    {
        public LearnFAMType_10Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.LearnerFAMs))
            {
                HandleValidationError(RuleNameConstants.LearnFAMType_10Rule, objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(IReadOnlyCollection<ILearnerFAM> learnerFams)
        {
            return learnerFams != null &&
                   learnerFams.Count(x => x.LearnFAMType == LearnerFamTypeConstants.LSR) > 4;
        }
    }
}