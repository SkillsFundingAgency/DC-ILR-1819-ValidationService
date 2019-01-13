using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.EmpOutcome
{
    public class EmpOutcome_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<int> _fundModels = new HashSet<int>()
        {
            TypeOfFunding.Age16To19ExcludingApprenticeships,
            TypeOfFunding.Other16To19,
            TypeOfFunding.CommunityLearning,
            TypeOfFunding.NotFundedByESFA,
        };

        public EmpOutcome_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.EmpOutcome_01)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.EmpOutcomeNullable, learningDelivery.FundModel))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.EmpOutcomeNullable));
                }
            }
        }

        public bool ConditionMet(int? empOutcome, int fundModel)
        {
            return empOutcome.HasValue && _fundModels.Contains(fundModel);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, int? empOutcome)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.EmpOutcome, empOutcome),
            };
        }
    }
}
