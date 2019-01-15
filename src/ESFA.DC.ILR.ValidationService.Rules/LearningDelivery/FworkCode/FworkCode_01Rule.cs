using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FworkCode
{
    public class FworkCode_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_07Rule _dd07;

        public FworkCode_01Rule(IDerivedData_07Rule dd07, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.FworkCode_01)
        {
            _dd07 = dd07;
        }

        public FworkCode_01Rule()
            : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FworkCodeNullable, learningDelivery.ProgTypeNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.ProgTypeNullable));
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
            return progType != 25 && _dd07.IsApprenticeship(progType);
        }

        public virtual bool FworkCodeConditionMet(int? fworkCode)
        {
            return !fworkCode.HasValue;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? progType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType)
            };
        }
    }
}
