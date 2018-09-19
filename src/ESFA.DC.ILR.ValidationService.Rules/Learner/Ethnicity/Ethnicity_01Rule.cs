using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.Ethnicity
{
    public class Ethnicity_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<long> _validEthnicityValues = new HashSet<long>() { 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 98, 99 };

        public Ethnicity_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.EthnicityNullable))
            {
                HandleValidationError(RuleNameConstants.Ethnicity_01Rule, objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(long? ethnicty)
        {
            return ethnicty.HasValue &&
                   !_validEthnicityValues.Contains(ethnicty.Value);
        }
    }
}