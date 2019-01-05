using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R105Rule : AbstractRule, IRule<ILearner>
    {
        public R105Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R105)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            if (LearnDelFAMTypeConditionMet(objectToValidate)
                && LearnDelFAMCodeConditionMet(objectToValidate))
            {
                HandleValidationError(objectToValidate.LearnRefNumber);
            }
        }

        public bool LearnDelFAMCodeConditionMet(ILearner learner)
        {
            return learner.LearningDeliveries?
                .Where(l => l.LearningDeliveryFAMs != null)?
                .SelectMany(l => l.LearningDeliveryFAMs)?
                .GroupBy(f => f.LearnDelFAMCode)?
                .Any(g => g.Key.Count() > 1) ?? false;
        }

        public bool LearnDelFAMTypeConditionMet(ILearner learner)
        {
            return (learner.LearningDeliveries?
                .Where(l => l.LearningDeliveryFAMs != null)?
                .SelectMany(l => l.LearningDeliveryFAMs)?
                .Where(f => f.LearnDelFAMType == LearningDeliveryFAMTypeConstants.ACT)?
                .Count() ?? 0) > 1;
        }
    }
}
