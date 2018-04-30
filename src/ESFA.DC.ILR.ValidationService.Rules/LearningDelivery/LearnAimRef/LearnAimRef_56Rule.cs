using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_56Rule : AbstractRule, IRule<ILearner>
    {
        public LearnAimRef_56Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_56)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            throw new System.NotImplementedException();
        }

        public bool ConditionMet()
        {
            return false;
        }

        public bool LearnStartDateConditionMet()
        {
            return false;
        }

        public bool FundModelConditionMet()
        {
            return false;
        }

        public bool LearningDeliveryFAMsConditionMet()
        {
            return false;
        }

        public bool LearningDeliveryCategoryConditionMet()
        {
            return false;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnAimRef, DateTime learnStartDate, int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
            };
        }
    }
}
