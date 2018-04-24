using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FundModel
{
    public class FundModel_07Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDD07 _dd07;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;

        private readonly DateTime _learnStartDate = new DateTime(2017, 5, 1);
        private readonly IEnumerable<string> _ldmLearningDeliveryFamCodes = new List<string>() { "353", "354", "355" };

        public FundModel_07Rule(IDD07 dd07, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.FundModel_07)
        {
            _dd07 = dd07;
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public FundModel_07Rule()
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
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel));
                }
            }
        }

        public bool ConditionMet(int aimType, int fundModel, DateTime learnStartDate, int? progType, IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return AimTypeConditionMet(aimType)
                   && FundModelConditionMet(fundModel)
                   && LearnStartDateConditionMet(learnStartDate)
                   && ApprenticeshipConditionMet(progType)
                   && LearningDeliveryFAMConditionMet(learningDeliveryFams);
        }

        public virtual bool AimTypeConditionMet(int aimType)
        {
            return aimType == 1;
        }

        public virtual bool FundModelConditionMet(int fundModel)
        {
            return fundModel != 36 && fundModel != 99;
        }

        public virtual bool ApprenticeshipConditionMet(int? progType)
        {
            return _dd07.Derive(progType) == ValidationConstants.Y;
        }

        public virtual bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate >= _learnStartDate;
        }

        public virtual bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return !_learningDeliveryFamQueryService.HasLearningDeliveryFAMType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.RES)
                && !_learningDeliveryFamQueryService.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.LDM, _ldmLearningDeliveryFamCodes);
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
