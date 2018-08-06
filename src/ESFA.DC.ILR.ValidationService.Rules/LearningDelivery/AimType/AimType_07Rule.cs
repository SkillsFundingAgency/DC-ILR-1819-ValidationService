using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AimType
{
    public class AimType_07Rule : AbstractRule, IRule<ILearner>
    {
        private const string _learnDelFamCode = "105";

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;
        private readonly DateTime _minimumLearnStartDate = new DateTime(2017, 8, 1);

        public AimType_07Rule(ILearningDeliveryFAMQueryService learningDeliveryFamQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AimType_07)
        {
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.ProgTypeNullable, learningDelivery.AimType, learningDelivery.LearnStartDate) && LearningDeliveryFAMConditionMet(learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.AimType, LearningDeliveryFAMTypeConstants.SOF, _learnDelFamCode));
                }
            }
        }

        public bool ConditionMet(int? progType, int aimType, DateTime learnStartDate)
        {
            return progType != 24
                   && aimType == 5
                   && learnStartDate >= _minimumLearnStartDate;
        }

        public bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return _learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.SOF, _learnDelFamCode);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int aimType, string learnDelFamType, string learnDelFamCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFamType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learnDelFamCode),
            };
        }
    }
}
