using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R07Rule : AbstractRule, IRule<ILearner>
    {
        private IList<int> _aimSeqNumbers;

        public R07Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R07)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.LearningDeliveries))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(_aimSeqNumbers));
            }
        }

        public bool ConditionMet(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            _aimSeqNumbers = learningDeliveries.GroupBy(ld => ld.AimSeqNumber)
                .Where(c => c.Count() > 1)
                .Select(k => k.Key)
                .ToList();

            return _aimSeqNumbers.Count > 0;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(IList<int> aimSeqNumbers)
        {
            var aimSeqNumberStrings = string.Join(", ", aimSeqNumbers.Select(a => a.ToString()));

            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimSeqNumber, aimSeqNumberStrings)
            };
        }
    }
}
