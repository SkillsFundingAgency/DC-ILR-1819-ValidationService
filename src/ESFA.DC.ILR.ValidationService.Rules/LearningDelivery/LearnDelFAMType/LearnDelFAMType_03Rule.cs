using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly IEnumerable<int> _fundModels = new HashSet<int>() { 10 };

        public LearnDelFAMType_03Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_03)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(d => _fundModels.Contains(d.FundModel)
                        && d.LearningDeliveryFAMs != null))
            {
                if (ConditionMet(learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel, LearningDeliveryFAMTypeConstants.ASL, "1"));
                }
            }
        }

        public bool ConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return LearningDeliveryFAMsConditionMet(learningDeliveryFAMs);
        }

        public bool LearningDeliveryFAMsConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ASL)
                && !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "340");
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, string learningDelFAMType, string learningDelFAMCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learningDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learningDelFAMCode)
            };
        }
    }
}
