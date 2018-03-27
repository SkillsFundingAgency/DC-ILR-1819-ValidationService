using System.Text.RegularExpressions;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.Postcode
{
    /// <summary>
    /// The first part of Learner.PostcodePrior <> valid format of 1 or 2 capital letters followed by (1 or 2 numbers or 1 number and a capital letter)
    /// and the second part of the postcode <> valid format of (nXX) where n = 0-9 and XX are capital letters excluding C, I, K, M, O and V
    /// </summary>
    public class Postcode_15Rule : AbstractRule, IRule<ILearner>
    {
        private readonly Regex _regex = new Regex("^[A-Z]{1,2}([0-9]{1,2}|[0-9][A-Z]) [0-9][ABD-HJLNP-UW-Z]{2}$", RegexOptions.Compiled);

        public Postcode_15Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.Postcode))
            {
                HandleValidationError(RuleNameConstants.PostCode_15Rule, objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(string postcode)
        {
            return !string.IsNullOrWhiteSpace(postcode) &&
                   _regex.IsMatch(postcode.Trim()) == false;
        }
    }
}