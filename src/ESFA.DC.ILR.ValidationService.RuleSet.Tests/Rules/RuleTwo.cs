using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Tests.Rules
{
    public class RuleTwo : IRule<string>
    {
        private readonly IValidationErrorCache<string> _validationErrorCache;

        public RuleTwo(IValidationErrorCache<string> validationErrorCache)
        {
            _validationErrorCache = validationErrorCache;
        }

        public string RuleName => "RuleTwo";

        public void Validate(string objectToValidate)
        {
            _validationErrorCache.Add("2");
            _validationErrorCache.Add("3");
        }
    }
}
