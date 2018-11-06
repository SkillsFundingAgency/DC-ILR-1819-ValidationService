using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.HEPostcode
{
    public class HEPostCode_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IPostcodeQueryService _postcodeQueryService;

        public HEPostCode_02Rule(
            IPostcodeQueryService postcodeQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.HEPostCode_02)
        {
            _postcodeQueryService = postcodeQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearningDeliveryHEEntity))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(learningDelivery.LearningDeliveryHEEntity.HEPostCode));
                }
            }
        }

        public bool ConditionMet(ILearningDeliveryHE learningDeliveryHEEntity)
        {
            return learningDeliveryHEEntity != null
                   && _postcodeQueryService.RegexValid(learningDeliveryHEEntity.HEPostCode) == false;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string postcode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.HEPostcode, postcode),
            };
        }
    }
}
