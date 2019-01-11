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
    public class R113Rule : AbstractRule, IRule<ILearner>
    {
        public R113Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R113)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (learningDelivery.LearningDeliveryFAMs != null
                    && ConditionMet(
                    learningDelivery.LearnActEndDateNullable,
                    learningDelivery.LearningDeliveryFAMs,
                    out string learnDelFAMType,
                    out DateTime? learnDelFAMDateTo))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learningDelivery.LearnActEndDateNullable,
                            learnDelFAMType,
                            learnDelFAMDateTo));
                }
            }
        }

        public bool ConditionMet(
            DateTime? learnActEndDateNullable,
            IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs,
            out string learnDelFAMType,
            out DateTime? learnDelFAMDateTo)
        {
            learnDelFAMType = string.Empty;
            learnDelFAMDateTo = null;

            return learnActEndDateNullable == null
                && LearningDeliveryFAMsConditionMet(
                    learningDeliveryFAMs,
                    out learnDelFAMType,
                    out learnDelFAMDateTo);
        }

        public bool LearningDeliveryFAMsConditionMet(
            IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs,
            out string learnDelFAMType,
            out DateTime? learnDelFAMDateTo)
        {
            var latestLearnDelFAMDate = learningDeliveryFAMs?
                .Where(f => f.LearnDelFAMType == LearningDeliveryFAMTypeConstants.ACT
                && f.LearnDelFAMDateFromNullable.HasValue)?
                .OrderByDescending(o => o.LearnDelFAMDateFromNullable)
                .FirstOrDefault();

            if (latestLearnDelFAMDate != null
                && latestLearnDelFAMDate.LearnDelFAMDateToNullable.HasValue)
            {
                learnDelFAMType = latestLearnDelFAMDate.LearnDelFAMType;
                learnDelFAMDateTo = latestLearnDelFAMDate.LearnDelFAMDateToNullable;
                return true;
            }

            learnDelFAMType = string.Empty;
            learnDelFAMDateTo = null;

            return false;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(
            DateTime? learnActEndDateNullable,
            string learnDelFAMType,
            DateTime? learnDelFAMDateTo)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDateNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, learnDelFAMDateTo)
            };
        }
    }
}
