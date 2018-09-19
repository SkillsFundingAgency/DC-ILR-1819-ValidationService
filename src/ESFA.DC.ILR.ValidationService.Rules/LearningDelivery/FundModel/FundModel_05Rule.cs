using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FundModel
{
    public class FundModel_05Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _learnStartDate = new DateTime(2017, 5, 1);

        public FundModel_05Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.FundModel_05)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FundModel, learningDelivery.LearnStartDate))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.LearnStartDate));
                }
            }
        }

        public bool ConditionMet(int fundModel, DateTime learnStartDate)
        {
            return fundModel == 36 && learnStartDate < _learnStartDate;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, DateTime learnStartDateTime)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDateTime),
            };
        }
    }
}
