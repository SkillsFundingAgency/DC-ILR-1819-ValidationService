using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R112Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public R112Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.R112)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearnActEndDateNullable, learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnActEndDateNullable, LearningDeliveryFAMTypeConstants.ACT, null));
                }
            }
        }

        public bool ConditionMet(DateTime? learnActEndDateNullable, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return learnActEndDateNullable.HasValue
                && LearningDeliveryFAMTypeConditionMet(learningDeliveryFAMs, learnActEndDateNullable);
        }

        public bool LearningDeliveryFAMTypeConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, DateTime? learnActEndDate)
        {
            ILearningDeliveryFAM learningDeliveryFAM =
                _learningDeliveryFAMQueryService.GetLearningDeliveryFAMByTypeAndLatestByDateFrom(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT);

            return learningDeliveryFAM != null
                && (!learningDeliveryFAM.LearnDelFAMDateToNullable.HasValue
                    || learningDeliveryFAM.LearnDelFAMDateToNullable != learnActEndDate);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? learnActEndDateNullable, string learnDelFAMType, DateTime? learnDelFAMDateTo)
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
