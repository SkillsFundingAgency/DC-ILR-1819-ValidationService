using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R114Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_31Rule _dd31;
        private readonly IDerivedData_32Rule _dd32;

        public R114Rule(IDerivedData_31Rule dd31, IDerivedData_32Rule dd32, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R114)
        {
            _dd31 = dd31;
            _dd32 = dd32;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            var earliestOpenApprenticeshipFundedLearningDelivery = GetEarliestOpenApprenticeshipFundedLearningDelivery(objectToValidate);

            if (earliestOpenApprenticeshipFundedLearningDelivery != null)
            {
                foreach (var adultSkillsEnglishOrMathsAim in GetAdultSkillsEnglishOrMathsAims(objectToValidate))
                {
                    if (DateConditionMet(adultSkillsEnglishOrMathsAim, earliestOpenApprenticeshipFundedLearningDelivery))
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            adultSkillsEnglishOrMathsAim.AimSeqNumber,
                            BuildErrorMessageParameters(adultSkillsEnglishOrMathsAim.AimType, adultSkillsEnglishOrMathsAim.LearnStartDate, adultSkillsEnglishOrMathsAim.FundModel, adultSkillsEnglishOrMathsAim.LearnActEndDateNullable));
                    }
                }
            }
        }

        public bool DateConditionMet(ILearningDelivery adultSkillsEnglishOrMathsAim, ILearningDelivery earliestOpenApprenticeshipFundedLearningDelivery)
        {
            return adultSkillsEnglishOrMathsAim.LearnStartDate >= earliestOpenApprenticeshipFundedLearningDelivery.LearnStartDate;
        }

        public ILearningDelivery GetEarliestOpenApprenticeshipFundedLearningDelivery(ILearner learner)
        {
            return learner?
                .LearningDeliveries?
                .Where(_dd32.IsOpenApprenticeshipFundedProgramme)
                .OrderBy(ld => ld.LearnStartDate)
                .FirstOrDefault();
        }

        public IEnumerable<ILearningDelivery> GetAdultSkillsEnglishOrMathsAims(ILearner learner)
        {
            return learner?
                .LearningDeliveries?
                .Where(_dd31.IsAdultSkillsFundedEnglishOrMathsAim)
                ?? Enumerable.Empty<ILearningDelivery>();
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int aimType, DateTime learnStartDate, int fundModel, DateTime? learnActEndDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate),
            };
        }
    }
}
