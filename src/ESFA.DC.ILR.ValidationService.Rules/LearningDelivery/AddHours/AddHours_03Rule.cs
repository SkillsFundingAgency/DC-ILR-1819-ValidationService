using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AddHours
{
    public class AddHours_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<int> _basicSkillsType = new HashSet<int>() { 36, 37, 38, 39, 40, 41, 42 };

        private readonly ILARSDataService _larsDataService;

        public AddHours_03Rule(ILARSDataService larsDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AddHours_03)
        {
            _larsDataService = larsDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (learningDelivery.AddHoursNullable != null)
                {
                    if (ConditionMet(learningDelivery.LearnAimRef, learningDelivery.LearnStartDate))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnAimRef, learningDelivery.AddHoursNullable));
                    }
                }
            }
        }

        public bool ConditionMet(string learnAimRef, DateTime learnStartDate)
        {
            return !_larsDataService.BasicSkillsMatchForLearnAimRefAndStartDate(_basicSkillsType, learnAimRef, learnStartDate);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnAimRef, int? addHoursNullable)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.AddHours, addHoursNullable)
            };
        }
    }
}
