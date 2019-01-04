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
    public class R52Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<string> _learningDeliveryFAMTypes = new HashSet<string>()
        {
            LearningDeliveryFAMTypeConstants.ACT,
            LearningDeliveryFAMTypeConstants.ALB,
            LearningDeliveryFAMTypeConstants.LSF,
        };

        public R52Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R52)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            if (ConditionMet(objectToValidate))
            {
                HandleValidationError(objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(ILearner learner)
        {
            return learner.LearningDeliveries
                .Where(l => l.LearningDeliveryFAMs != null)?
                .SelectMany(l => l.LearningDeliveryFAMs)
                .Where(l => !_learningDeliveryFAMTypes.Contains(l.LearnDelFAMType))?
                .GroupBy(l => new { l.LearnDelFAMType, l.LearnDelFAMCode })
                .Any(g => g.Count() > 1) ?? false;
        }
    }
}
