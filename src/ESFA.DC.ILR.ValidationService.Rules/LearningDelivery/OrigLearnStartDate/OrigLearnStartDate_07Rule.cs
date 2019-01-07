using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate
{
    public class OrigLearnStartDate_07Rule : AbstractRule, IRule<ILearner>
    {
        private const int FundModel81 = 81;
        private const int FundModel36 = 36;
        private const int ProgType25 = 25;

        private readonly ILARSDataService _larsDataService;

        public OrigLearnStartDate_07Rule(
            ILARSDataService larsDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.OrigLearnStartDate_07)
        {
            _larsDataService = larsDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.OrigLearnStartDateNullable,
                    learningDelivery.FundModel,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.LearnAimRef))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.OrigLearnStartDateNullable));
                }
            }
        }

        public bool ConditionMet(DateTime? origLearnStartDate, int fundModel, int? progType, string learnAimRef)
        {
            return OrigLearnStartDateConditionMet(origLearnStartDate)
                   && FundModelConditionMet(fundModel, progType)
                   && LARSConditionMet(origLearnStartDate, learnAimRef);
        }

        public bool OrigLearnStartDateConditionMet(DateTime? origLearnStartDate)
        {
            return origLearnStartDate.HasValue;
        }

        public bool FundModelConditionMet(int fundModel, int? progType)
        {
            return fundModel == FundModel81 || (fundModel == FundModel36 && progType.HasValue && progType == ProgType25);
        }

        public bool LARSConditionMet(DateTime? origLearnStartDate, string learnAimRef)
        {
            return !_larsDataService.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, TypeOfLARSValidity.Any);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? origLearnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.OrigLearnStartDate, origLearnStartDate)
            };
        }
    }
}
