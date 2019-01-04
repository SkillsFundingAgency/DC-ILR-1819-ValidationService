using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Tests.Rules
{
    public class RuleOne : IRule<string>
    {
        private readonly IValidationErrorCache<string> _validationErrorCache;

        public RuleOne(IValidationErrorCache<string> validationErrorCache)
        {
            _validationErrorCache = validationErrorCache;
        }

        public string RuleName => "RuleOne";

        public void Validate(string objectToValidate)
        {
            _validationErrorCache.Add("1");
        }
    }
}
