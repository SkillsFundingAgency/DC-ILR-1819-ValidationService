using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ULN
{
    public class ULN_04Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_01Rule _dd01;

        public ULN_04Rule(IDerivedData_01Rule dd01, IValidationErrorHandler validationErrorHandler)
             : base(validationErrorHandler, RuleNameConstants.ULN_04)
        {
            _dd01 = dd01;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.ULN, _dd01.Derive(objectToValidate.ULN)))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.ULN));
                return;
            }
        }

        public bool ConditionMet(long? uln, string dd01)
        {
            if (!uln.HasValue)
            {
                return true;
            }

            var ulnString = uln.ToString();

            return dd01 == ValidationConstants.N
                   || (dd01 != ValidationConstants.Y && ulnString.Length >= 10 && dd01 != ulnString.ElementAt(9).ToString());
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