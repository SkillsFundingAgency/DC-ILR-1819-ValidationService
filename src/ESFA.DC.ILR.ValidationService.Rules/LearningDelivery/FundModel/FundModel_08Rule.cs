using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FundModel
{
    public class FundModel_08Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDD07 _dd07;

        private readonly DateTime _learnStartDate = new DateTime(2017, 5, 1);

        public FundModel_08Rule(IDD07 dd07, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.FundModel_08)
        {
            _dd07 = dd07;
        }

        public FundModel_08Rule()
            : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.AimType,
                    learningDelivery.FundModel,
                    learningDelivery.LearnStartDate,
                    learningDelivery.ProgTypeNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel));
                }
            }
        }

        public bool ConditionMet(int aimType, int fundModel, DateTime learnStartDate, int? progType)
        {
            return AimTypeConditionMet(aimType)
                   && FundModelConditionMet(fundModel)
                   && LearnStartDateConditionMet(learnStartDate)
                   && ApprenticeshipConditionMet(progType);
        }

        public virtual bool AimTypeConditionMet(int aimType)
        {
            return aimType == 1;
        }

        public virtual bool FundModelConditionMet(int fundModel)
        {
            return fundModel != 35 && fundModel != 99 && fundModel != 81;
        }

        public virtual bool ApprenticeshipConditionMet(int? progType)
        {
            return progType != 25 && _dd07.IsApprenticeship(progType);
        }

        public virtual bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate < _learnStartDate;
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
