using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType
{
    /// <summary>
    /// If a Learner FAM Type of Special Educational Needs (SEN) is recorded, the learner must not have a record of FAM type EHC or LDA as well
    /// </summary>
    public class LearnFAMType_14Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnerFAMQueryService _learnerFAMQueryService;

        public LearnFAMType_14Rule(IValidationErrorHandler validationErrorHandler, ILearnerFAMQueryService learnerFAMQueryService)
            : base(validationErrorHandler)
        {
            _learnerFAMQueryService = learnerFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.LearnerFAMs))
            {
                HandleValidationError(RuleNameConstants.LearnFAMType_14Rule, objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(IReadOnlyCollection<ILearnerFAM> learnerFams)
        {
            return _learnerFAMQueryService.HasLearnerFAMCodeForType(learnerFams, LearnerFamTypeConstants.SEN, 1) &&
                   _learnerFAMQueryService.HasLearnerFAMCodeForType(learnerFams, LearnerFamTypeConstants.EHC, 1);
        }
    }
}