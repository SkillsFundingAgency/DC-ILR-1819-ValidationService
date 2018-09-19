using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.DelLocPostCode
{
    public class DelLocPostCode_11Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IPostcodeQueryService _postcodeQueryService;

        public DelLocPostCode_11Rule(IPostcodeQueryService postcodeQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DelLocPostCode_11)
        {
            _postcodeQueryService = postcodeQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.DelLocPostCode))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.DelLocPostCode));
                }
            }
        }

        public bool ConditionMet(string postcode)
        {
            return !_postcodeQueryService.RegexValid(postcode);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string postcode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DelLocPostCode, postcode)
            };
        }
    }
}
