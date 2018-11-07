﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.MathGrade
{
    public class MathGrade_01Rule : AbstractRule, IRule<ILearner>
    {
        public MathGrade_01Rule(
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.MathGrade_01)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (MathGradeConditionMet(objectToValidate.MathGrade))
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (FundModelConditionMet(learningDelivery.FundModel))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel));
                        return;
                    }
                }
            }
        }

        public bool MathGradeConditionMet(string mathGrade)
        {
            return string.IsNullOrWhiteSpace(mathGrade);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            var fundModels = new[] { 25, 82 };

            return fundModels.Contains(fundModel);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
            };
        }
    }
}
