using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.Sex
{
    /// <summary>
    /// The learner's Sex must be a valid lookup
    /// </summary>
    public class Sex_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<string> _validSexValues = new HashSet<string>() { "M", "F" };

        public Sex_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.Sex))
            {
                HandleValidationError(RuleNameConstants.Sex_01Rule, objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(string sex)
        {
            return !string.IsNullOrWhiteSpace(sex) && !_validSexValues.Contains(sex);
        }
    }
}