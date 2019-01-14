using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_46Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public LearnDelFAMType_46Rule(
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_46)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(d => d.LearningDeliveryFAMs != null))
            {
                foreach (var learnDelFam in learningDelivery.LearningDeliveryFAMs)
                {
                    if (ConditionMet(learningDelivery.FundModel, learnDelFam.LearnDelFAMType))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learnDelFam.LearnDelFAMType));
                    }
                }
            }
        }

        public bool ConditionMet(int fundModel, string learnDelFamType)
        {
            return FAMTypeConditionMet(learnDelFamType)
                   && FundModelConditionMet(fundModel);
        }

        public bool FAMTypeConditionMet(string learnDelFamType)
        {
            return learnDelFamType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.FLN);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel != TypeOfFunding.AdultSkills;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnDelFAMType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFAMType)
            };
        }
    }
}
