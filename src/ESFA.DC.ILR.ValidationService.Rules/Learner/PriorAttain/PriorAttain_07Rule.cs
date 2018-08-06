using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain
{
    // <summary>
    // LearningDelivery.LearnStartDate > 2016-07-31 and LearningDelivery.FundModel = 35 and
    // Learner.PriorAttain = (3, 4, 5, 10, 11, 12, 13, 97 or 98) and LearningDelivery.ProgType = 24
    // </summary>
    public class PriorAttain_07Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<long> _validPriorAttainValues = new HashSet<long> { 4, 5, 10, 11, 12, 13, 97, 98 };
        private readonly DateTime _startConditionDate = new DateTime(2016, 7, 31);

        public PriorAttain_07Rule(IValidationErrorHandler validationErrorHandler)
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
                    HandleValidationError(RuleNameConstants.PriorAttain_07Rule, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
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
            return learnStartDate.HasValue && learnStartDate.Value > _startConditionDate;
        }

        public bool FundModelConditionMet(long? fundModel)
        {
            return fundModel.HasValue && fundModel.Value == 35;
        }

        public bool ProgTypeConditionMet(long? progType)
        {
            return progType.HasValue && progType.Value == 24;
        }
    }
}