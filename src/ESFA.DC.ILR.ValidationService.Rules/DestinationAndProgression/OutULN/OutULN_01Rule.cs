using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutULN
{
    public class OutULN_01Rule : AbstractRule, IRule<ILearnerDestinationAndProgression>
    {
        private readonly IDerivedData_15Rule _derivedData15Rule;

        public OutULN_01Rule(
            IDerivedData_15Rule derivedData15Rule,
            IValidationErrorHandler validationErrorHandler)
          : base(validationErrorHandler, RuleNameConstants.OutULN_01)
        {
            _derivedData15Rule = derivedData15Rule;
        }

        public void Validate(ILearnerDestinationAndProgression objectToValidate)
        {
            if (ConditionMet(objectToValidate.ULN))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.ULN));
            }
        }

        private bool ConditionMet(long uln)
        {
            var result = _derivedData15Rule.Derive(uln);
            return result == ValidationConstants.N || (result != ValidationConstants.Y && result != uln.ToString().Last().ToString());
        }

        private IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(long uln)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.OutULN, uln)
            };
        }
    }
}
