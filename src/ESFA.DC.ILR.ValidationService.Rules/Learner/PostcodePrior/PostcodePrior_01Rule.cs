using System.Collections.Generic;
using System.Text.RegularExpressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PostcodePrior
{
    public class PostcodePrior_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IPostcodesDataService _postcodesDataService;

        public PostcodePrior_01Rule(IPostcodesDataService postcodesDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PostcodePrior_01)
        {
            _postcodesDataService = postcodesDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.PostcodePrior))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.PostcodePrior));
            }
        }

        public bool ConditionMet(string postcodePrior)
        {
            return NullConditionMet(postcodePrior)
                && TemporaryPostcodeConditionMet(postcodePrior)
                && PostcodeConditionMet(postcodePrior);
        }

        public bool NullConditionMet(string postcodePrior)
        {
            return !string.IsNullOrWhiteSpace(postcodePrior);
        }

        public bool TemporaryPostcodeConditionMet(string postcodePrior)
        {
            return postcodePrior != ValidationConstants.TemporaryPostCode;
        }

        public bool PostcodeConditionMet(string postcodePrior)
        {
            return !_postcodesDataService.PostcodeExists(postcodePrior);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string postcodePrior)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PostcodePrior, postcodePrior)
            };
        }
    }
}