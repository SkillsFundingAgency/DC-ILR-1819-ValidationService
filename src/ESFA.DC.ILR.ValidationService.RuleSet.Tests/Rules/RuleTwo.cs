using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Tests.Rules
{
    public class RuleTwo : IRule<string>
    {
        public string RuleName
        {
            get { return string.Empty; }
        }

        public void Validate(string objectToValidate)
        {
        }
    }
}
