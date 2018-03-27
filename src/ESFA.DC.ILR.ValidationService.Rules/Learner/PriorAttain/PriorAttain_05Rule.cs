using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain
{
    /// <summary>
    /// If the prior attainment is level 4 or above, then the learner must not be on an Adult skills funded intermediate or advanced apprenticeship
    /// </summary>
    public class PriorAttain_05Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<long> _validPriorAttainValues = new HashSet<long> { 4, 5, 10, 11, 12, 13 };
        private readonly DateTime _startConditionDate = new DateTime(2015, 8, 1);

        public PriorAttain_05Rule(IValidationErrorHandler validationErrorHandler)
           : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    objectToValidate.PriorAttainNullable,
                    learningDelivery.FundModelNullable,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.LearnStartDateNullable))
                {
                    HandleValidationError(RuleNameConstants.PriorAttain_05Rule, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
                }
            }
        }

        public bool ConditionMet(long? priorAttain, long? fundModel, long? progType, DateTime? learnStartDate)
        {
            return PriorAttainConditionMet(priorAttain) &&
                    LearnStartDateConditionMet(learnStartDate) &&
                    FundModelConditionMet(fundModel) &&
                    ProgTypeConditionMet(progType);
        }

        public bool PriorAttainConditionMet(long? priorAttain)
        {
            return priorAttain.HasValue && _validPriorAttainValues.Contains(priorAttain.Value);
        }

        public bool LearnStartDateConditionMet(DateTime? learnStartDate)
        {
            return learnStartDate.HasValue && learnStartDate.Value >= _startConditionDate;
        }

        public bool FundModelConditionMet(long? fundModel)
        {
            return fundModel.HasValue && fundModel.Value == 35;
        }

        public bool ProgTypeConditionMet(long? progType)
        {
            return progType.HasValue && progType.Value == 20;
        }
    }
}