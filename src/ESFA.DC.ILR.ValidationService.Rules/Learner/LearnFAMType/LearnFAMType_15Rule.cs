using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType
{
    /// <summary>
    /// If the learner is exempt from maths GCSE condition of funding due to learning difficulties or disabilities, they must have Special Educational Needs (SEN)  or Education health care plan (EHC plan)
    /// </summary>
    public class LearnFAMType_15Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnerFAMQueryService _learnerFAMQueryService;
        private readonly IEnumerable<string> _senOrEhcFamStrings = new[] { LearnerFamTypeConstants.SEN, LearnerFamTypeConstants.EHC };

        public LearnFAMType_15Rule(IValidationErrorHandler validationErrorHandler, ILearnerFAMQueryService learnerFAMQueryService)
            : base(validationErrorHandler)
        {
            _learnerFAMQueryService = learnerFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.LearnerFAMs))
            {
                HandleValidationError(RuleNameConstants.LearnFAMType_15Rule, objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(IReadOnlyCollection<ILearnerFAM> learnerFams)
        {
            return ConditionMetForValidFamType(learnerFams) &&
                   ConditionMetSENOrEHCNotFound(learnerFams);
        }

        public bool ConditionMetSENOrEHCNotFound(IReadOnlyCollection<ILearnerFAM> learnerFams)
        {
            return !_learnerFAMQueryService.HasAnyLearnerFAMTypes(learnerFams, _senOrEhcFamStrings);
        }

        public bool ConditionMetForValidFamType(IReadOnlyCollection<ILearnerFAM> learnerFams)
        {
            return _learnerFAMQueryService.HasLearnerFAMCodeForType(learnerFams, LearnerFamTypeConstants.MCF, 1);
        }
    }
}