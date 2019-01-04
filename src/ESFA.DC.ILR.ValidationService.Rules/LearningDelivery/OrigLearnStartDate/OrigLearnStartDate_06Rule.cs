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
    public class OrigLearnStartDate_06Rule : AbstractRule, IRule<ILearner>
    {
        private const int FundModel35 = 35;

        private readonly HashSet<string> LarsValidityCategories = new HashSet<string>()
        {
            TypeOfLARSValidity.AdultSkills,
            TypeOfLARSValidity.Unemployed,
            TypeOfLARSValidity.OLASSAdult
        };

        private readonly IDD07 _dd07;
        private readonly ILARSDataService _larsDataService;

        public OrigLearnStartDate_06Rule(
            IDD07 dd07,
            ILARSDataService larsDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.OrigLearnStartDate_06)
        {
            _dd07 = dd07;
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
                        BuildErrorMessageParameters(learningDelivery.OrigLearnStartDateNullable, learningDelivery.FundModel, learningDelivery.AimType));
                }
            }
        }

        public bool ConditionMet(DateTime? origLearnStartDate, int fundModel, int? progType, string learnAimRef)
        {
            return OrigLearnStartDateConditionMet(origLearnStartDate)
                   && FundModelConditionMet(fundModel)
                   && !Excluded(progType)
                   && LARSConditionMet(origLearnStartDate, learnAimRef);
        }

        public bool OrigLearnStartDateConditionMet(DateTime? origLearnStartDate)
        {
            return origLearnStartDate.HasValue;
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == FundModel35;
        }

        public bool LARSConditionMet(DateTime? origLearnStartDate, string learnAimRef)
        {
            return !_larsDataService.OrigLearnStartDateBetweenStartAndEndDateForAnyValidityCategory(origLearnStartDate, learnAimRef, LarsValidityCategories);
        }

        public bool Excluded(int? progType)
        {
            return _dd07.IsApprenticeship(progType);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? origLearnStartDate, int fundModel, int aimType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.OrigLearnStartDate, origLearnStartDate)
            };
        }
    }
}
