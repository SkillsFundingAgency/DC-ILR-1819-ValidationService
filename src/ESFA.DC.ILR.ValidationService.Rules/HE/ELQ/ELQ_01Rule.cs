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

namespace ESFA.DC.ILR.ValidationService.Rules.HE.ELQ
{
    public class ELQ_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        private readonly IEnumerable<long> _fundModels = new HashSet<long> { 99 };

        public ELQ_01Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ELQ_01)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(d => d.LearningDeliveryFAMs != null))
            {
                if (ConditionMet(learningDelivery.FundModel, learningDelivery.LearningDeliveryHEEntity, learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel, LearningDeliveryFAMTypeConstants.SOF, "1"));
                }
            }
        }

        public bool ConditionMet(int fundModel, ILearningDeliveryHE learningDeliveryHE, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _fundModels.Contains(fundModel)
                && LearningDeliveryHEConditionMet(learningDeliveryHE)
                && _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF, "1");
        }

        public bool LearningDeliveryHEConditionMet(ILearningDeliveryHE learningDeliveryHE)
        {
            return learningDeliveryHE != null
                && learningDeliveryHE.ELQNullable == null;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, string learningDeliveryFAMType, string learningDeliveryFAMCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learningDeliveryFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learningDeliveryFAMCode)
            };
        }
    }
}
