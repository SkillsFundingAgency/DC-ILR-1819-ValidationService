using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.Postcode
{
    public class Postcode_14Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IPostcodesDataService _postcodesDataService;

        public Postcode_14Rule(IPostcodesDataService postcodesDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.Postcode_14)
        {
            _postcodesDataService = postcodesDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.Postcode))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.Postcode));
            }
        }

        public bool ConditionMet(string postcode)
        {
            return NullConditionMet(postcode)
                && TemporaryPostcodeConditionMet(postcode)
                && PostcodeConditionMet(postcode);
        }

        public bool NullConditionMet(string postcode)
        {
            return !string.IsNullOrWhiteSpace(postcode);
        }

        public bool TemporaryPostcodeConditionMet(string postcode)
        {
            return postcode != ValidationConstants.TemporaryPostCode;
        }

        public bool PostcodeConditionMet(string postcode)
        {
            return !_postcodesDataService.PostcodeExists(postcode);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string postcode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.Postcode, postcode)
            };
        }
    }
}