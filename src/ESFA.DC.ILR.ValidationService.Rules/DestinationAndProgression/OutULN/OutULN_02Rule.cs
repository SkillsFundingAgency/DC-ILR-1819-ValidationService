using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.ULN.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutULN
{
    public class OutULN_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IULNDataService _ulnDataService;

        public OutULN_02Rule(IULNDataService ulnDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.OutULN_02)
        {
            _ulnDataService = ulnDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            var uln = objectToValidate.ULN;

            if (ConditionMet(uln))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.ULN));
            }
        }

        private bool ConditionMet(long? uln)
        {
            return uln != ValidationConstants.TemporaryULN && !_ulnDataService.Exists(uln);
        }

        private IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(long uln)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ULN, uln),
            };
        }
    }
}