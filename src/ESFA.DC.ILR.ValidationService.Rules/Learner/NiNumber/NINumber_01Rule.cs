using System.Collections.Generic;
using System.Text.RegularExpressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.NiNumber
{
    /// <summary>
    /// If the NI Number is returned, the first character must not be D, F, I, Q, U or V and
    /// the second character must not be D, F, I, O, Q, U or V and characters
    /// 3 to 8 must be numeric and character 9 must be A, B, C, D or space
    /// </summary>
    public class NINumber_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly Regex _regex = new Regex("^[A-CEGHJ-PR-TW-Z][A-CEGHJ-NPR-TW-Z]{1}[0-9]{6}[A-D]{0,1}$", RegexOptions.Compiled);

        public NINumber_01Rule(IValidationErrorHandler validationErrorHandler)
           : base(validationErrorHandler, RuleNameConstants.NINumber_01)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.NINumber))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.NINumber));
            }
        }

        public bool ConditionMet(string niNumber)
        {
            return !string.IsNullOrWhiteSpace(niNumber) &&
                    _regex.IsMatch(niNumber.Trim()) == false;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string niNumber)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.NINumber, niNumber),
            };
        }
    }
}