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
        private readonly IDD01 _dd01;

        public ULN_04Rule(IDD01 dd01, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
            _dd01 = dd01;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.ULNNullable, _dd01.Derive(objectToValidate.ULNNullable)))
            {
                HandleValidationError(RuleNameConstants.ULN_04, objectToValidate.LearnRefNumber);
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
    }
}