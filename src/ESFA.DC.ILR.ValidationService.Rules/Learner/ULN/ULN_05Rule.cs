using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.ULN.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ULN
{
    public class ULN_05Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IULNDataService _ulnDataService;

        public ULN_05Rule(IULNDataService ulnDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ULN_05)
        {
            _ulnDataService = ulnDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            var uln = objectToValidate.ULN;

            if (ConditionMet(uln, _ulnDataService.Exists(uln)))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.ULN));
                return;
            }
        }

        public bool ConditionMet(long? uln, bool ulnExists)
        {
            return !ulnExists && uln != ValidationConstants.TemporaryULN;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(long uln)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ULN, uln),
            };
        }
    }
}