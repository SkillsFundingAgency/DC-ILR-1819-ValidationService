﻿using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain
{
    public class PriorAttain_04Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int[] _validPriorAttains =
        {
            TypeOfPriorAttainment.Level4Expired20130731,
            TypeOfPriorAttainment.Level5AndAboveExpired20130731,
            TypeOfPriorAttainment.Level4,
            TypeOfPriorAttainment.Level5,
            TypeOfPriorAttainment.Level6,
            TypeOfPriorAttainment.Level7AndAbove
        };

        public PriorAttain_04Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PriorAttain_04)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    objectToValidate.PriorAttainNullable,
                    learningDelivery.FundModel,
                    learningDelivery.ProgTypeNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber);
                }
            }
        }

        public bool ConditionMet(int? priorAttain, int fundModel, int? progType)
        {
            return PriorAttainConditionMet(priorAttain)
                   && FundModelConditionMet(fundModel)
                   && ProgTypeConditionMet(progType);
        }

        public bool PriorAttainConditionMet(int? priorAttain)
        {
            return priorAttain.HasValue
                && _validPriorAttains.Contains(priorAttain.Value);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == 35;
        }

        public bool ProgTypeConditionMet(int? progType)
        {
            int[] progTypes = { 2, 3 };

            return progType.HasValue
                   && progTypes.Contains(progType.Value);
        }
    }
}
