using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMDateTo
{
    public class LearnDelFAMDateTo_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<int> _fundModels = new HashSet<int>()
        {
            FundModelConstants.Apprenticeships,
            FundModelConstants.AdultSkills,
            FundModelConstants.OtherAdult,
            FundModelConstants.NonFunded,
        };

        public LearnDelFAMDateTo_03Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMDateTo_03)
        {
        }

        public LearnDelFAMDateTo_03Rule()
            : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (learningDelivery.LearningDeliveryFAMs != null)
                {
                    foreach (var learningDeliveryFam in learningDelivery.LearningDeliveryFAMs)
                    {
                        if (ConditionMet(learningDeliveryFam.LearnDelFAMDateToNullable, learningDelivery.LearnActEndDateNullable, learningDelivery.FundModel))
                        {
                            HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnActEndDateNullable, learningDeliveryFam.LearnDelFAMDateToNullable));
                        }
                    }
                }
            }
        }

        public bool ConditionMet(DateTime? learnDelFamDateTo, DateTime? learnActEndDate, int fundModel)
        {
            return DatesConditionMet(learnDelFamDateTo, learnActEndDate)
                   && FundModelConditionMet(fundModel);
        }

        public virtual bool DatesConditionMet(DateTime? learnDelFamDateTo, DateTime? learnActEndDate)
        {
            return learnDelFamDateTo.HasValue && learnDelFamDateTo > learnActEndDate;
        }

        public virtual bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? learnActEndDate, DateTime? learnDelFamDateTo)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, learnDelFamDateTo)
            };
        }
    }
}
