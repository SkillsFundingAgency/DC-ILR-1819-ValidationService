using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType
{
    public class LearnFAMType_14Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnerFAMQueryService _learnerFAMQueryService;

        public LearnFAMType_14Rule(ILearnerFAMQueryService learnerFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnFAMType_14)
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
                    errorMessageParameters: BuildErrorMessageParameters(LearnerFAMTypeConstants.SEN, "1"));
            }
        }

        public bool ConditionMet(IEnumerable<ILearnerFAM> learnerFAMs)
        {
            return LearnerFAMsConditionMet(learnerFAMs);
        }

        public bool LearnerFAMsConditionMet(IEnumerable<ILearnerFAM> learnerFAMs)
        {
            return _learnerFAMQueryService.HasLearnerFAMCodeForType(learnerFAMs, LearnerFAMTypeConstants.SEN, 1)
                   && _learnerFAMQueryService.HasLearnerFAMCodeForType(learnerFAMs, LearnerFAMTypeConstants.EHC, 1);
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
