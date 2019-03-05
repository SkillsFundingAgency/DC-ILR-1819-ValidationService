using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.NiNumber
{
    public class NINumber_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;

        public NINumber_02Rule(
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.NINumber_02)
        {
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (NiNumberConditionMet(objectToValidate.NINumber))
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (ConditionMet(learningDelivery.LearningDeliveryFAMs))
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            errorMessageParameters: BuildErrorMessageParameters(objectToValidate.NINumber));
                    }
                }
            }
        }

        public bool NiNumberConditionMet(string niNumber)
        {
            return string.IsNullOrWhiteSpace(niNumber);
        }

        public bool ConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "ACT", "1");
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string niNumber)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.NINumber, niNumber)
            };
        }
    }
}
