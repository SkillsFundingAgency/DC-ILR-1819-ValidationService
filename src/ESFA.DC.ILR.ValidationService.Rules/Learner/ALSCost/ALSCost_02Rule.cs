using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ALSCost
{
    /// <summary>
    /// Learner.ALSCost not null and LearnerFAM.LearnFAMType not HNS
    /// </summary>
    public class ALSCost_02Rule : AbstractRule, IRule<ILearner>
    {
        private const string HnsFamCode = "HNS";
        private readonly ILearnerFAMQueryService _learnerFamQueryService;

        public ALSCost_02Rule(ILearnerFAMQueryService learnerFamQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
            _learnerFamQueryService = learnerFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.ALSCostNullable, objectToValidate.LearnerFAMs))
            {
                HandleValidationError(RuleNameConstants.ALSCost_02Rule, objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(long? aLsCostNullable, IReadOnlyCollection<ILearnerFAM> learnerFams)
        {
            return aLsCostNullable.HasValue
                && !_learnerFamQueryService.HasLearnerFAMType(learnerFams, HnsFamCode);
        }
    }
}