using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType
{
    public class LearnFAMType_16Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnerFAMQueryService _learnerFAMQueryService;
        private readonly IEnumerable<string> _learnerFAMTypes = new HashSet<string>() { LearnerFAMTypeConstants.SEN, LearnerFAMTypeConstants.EHC };

        public LearnFAMType_16Rule(ILearnerFAMQueryService learnerFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnFAMType_16)
        {
            _learnerFAMQueryService = learnerFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearnerFAMs == null)
            {
                return;
            }

            if (ConditionMet(objectToValidate.LearnerFAMs))
            {
                HandleValidationError(
                    learnRefNumber: objectToValidate.LearnRefNumber,
                    errorMessageParameters: BuildErrorMessageParameters(LearnerFAMTypeConstants.ECF, "1"));
            }
        }

        public bool ConditionMet(IEnumerable<ILearnerFAM> learnerFAMs)
        {
            return LearnerFAMsConditionMet(learnerFAMs);
        }

        public bool LearnerFAMsConditionMet(IEnumerable<ILearnerFAM> learnerFAMs)
        {
            return _learnerFAMQueryService.HasLearnerFAMCodeForType(learnerFAMs, LearnerFAMTypeConstants.ECF, 1)
                && !_learnerFAMQueryService.HasAnyLearnerFAMTypes(learnerFAMs, _learnerFAMTypes);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnFAMType, string learnFAMCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, learnFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnFAMCode, learnFAMCode)
            };
        }
    }
}
