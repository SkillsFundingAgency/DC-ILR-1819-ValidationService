using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FworkCode
{
    public class FworkCode_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<int?> _progTypes = new HashSet<int?>() { null, 24, 25, };

        public FworkCode_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.FworkCode_02)
        {
        }

        public FworkCode_02Rule()
            : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FworkCodeNullable, learningDelivery.ProgTypeNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FworkCodeNullable));
                }
            }
        }

        public bool ConditionMet(int? fworkCode, int? progType)
        {
            return FworkCodeConditionMet(fworkCode)
                   && ApprenticeshipConditionMet(progType);
        }

        public virtual bool ApprenticeshipConditionMet(int? progType)
        {
            return _progTypes.Contains(progType);
        }

        public virtual bool FworkCodeConditionMet(int? fworkCode)
        {
            return fworkCode.HasValue;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? fworkCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FworkCode, fworkCode)
            };
        }
    }
}
