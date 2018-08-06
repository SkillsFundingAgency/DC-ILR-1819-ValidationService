using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.EmpOutcome.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.EmpOutcome
{
    public class EmpOutcome_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEmpOutcomeDataService _empOutcomeDataService;

        public EmpOutcome_02Rule(IEmpOutcomeDataService empOutcomeDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.EmpOutcome_02)
        {
            _empOutcomeDataService = empOutcomeDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.EmpOutcomeNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.EmpOutcomeNullable));
                }
            }
        }

        public bool ConditionMet(int? empOutcome)
        {
            return empOutcome.HasValue && !_empOutcomeDataService.Exists(empOutcome.Value);
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
