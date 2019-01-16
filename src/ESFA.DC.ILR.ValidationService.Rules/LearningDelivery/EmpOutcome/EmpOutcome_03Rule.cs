using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.EmpOutcome
{
    public class EmpOutcome_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_07Rule _dd07;

        public EmpOutcome_03Rule(IDerivedData_07Rule dd07, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.EmpOutcome_03)
        {
            _dd07 = dd07;
        }

        public EmpOutcome_03Rule()
            : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.EmpOutcomeNullable, learningDelivery.ProgTypeNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber);
                }
            }
        }

        public bool ConditionMet(int? empOutcome, int? progType)
        {
            return EmpOutcomeConditionMet(empOutcome) && ApprenticeshipConditionMet(progType);
        }

        public virtual bool ApprenticeshipConditionMet(int? progType)
        {
            return _dd07.IsApprenticeship(progType);
        }

        public virtual bool EmpOutcomeConditionMet(int? empOutcome)
        {
            return empOutcome.HasValue;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? empOutcome)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.EmpOutcome, empOutcome)
            };
        }
    }
}
