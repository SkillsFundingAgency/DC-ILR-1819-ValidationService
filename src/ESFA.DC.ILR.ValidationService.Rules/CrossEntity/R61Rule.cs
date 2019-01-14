using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R61Rule : AbstractRule, IRule<ILearner>
    {
        public R61Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R61)
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
                var latestRecord = GetLatestLSFFam(learningDelivery.LearningDeliveryFAMs);

                if (ConditionMet(learningDelivery.LearningDeliveryFAMs, latestRecord))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(latestRecord));
                }
            }
        }

        public bool ConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams, ILearningDeliveryFAM learningDeliveryFam)
        {
            if (learningDeliveryFam == null)
            {
                return false;
            }

            return learningDeliveryFams.Any(x =>
                    x.LearnDelFAMDateToNullable.HasValue &&
                    learningDeliveryFam.LearnDelFAMDateFromNullable < x.LearnDelFAMDateToNullable.Value);
        }

        public ILearningDeliveryFAM GetLatestLSFFam(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            var latestLSFRecord = learningDeliveryFams?
                .Where(x => x.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.LSF) && x.LearnDelFAMDateFromNullable.HasValue)
                .OrderByDescending(x => x.LearnDelFAMDateFromNullable)
                .FirstOrDefault();

            return latestLSFRecord;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(ILearningDeliveryFAM learningDeliveryFam)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learningDeliveryFam.LearnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateFrom, learningDeliveryFam.LearnDelFAMDateFromNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, learningDeliveryFam.LearnDelFAMDateToNullable)
            };
        }
    }
}
