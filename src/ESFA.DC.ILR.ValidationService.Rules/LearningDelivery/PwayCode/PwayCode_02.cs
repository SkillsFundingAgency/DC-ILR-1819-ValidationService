﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PwayCode
{
    public class PwayCode_02 : AbstractRule, IRule<ILearner>
    {
        private readonly IDD07 _dd07;

        public PwayCode_02(
            IDD07 dd07,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PwayCode_02)
        {
            _dd07 = dd07;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.PwayCodeNullable,
                    learningDelivery.ProgTypeNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(learningDelivery.PwayCodeNullable, learningDelivery.ProgTypeNullable));
                }
            }
        }

        public bool ConditionMet(int? pwayCode, int? progType)
        {
            return PwayCodeConditionMet(pwayCode)
                   && ApprenticeshipConditionMet(progType);
        }

        public bool PwayCodeConditionMet(int? pwayCode)
        {
            return pwayCode.HasValue;
        }

        public bool ApprenticeshipConditionMet(int? progType)
        {
            return !_dd07.IsApprenticeship(progType)
                   || progType == 25;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? pwayCode, int? progType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PwayCode, pwayCode),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
            };
        }
    }
}
