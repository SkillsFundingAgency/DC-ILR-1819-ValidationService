namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType
{
    using System.Collections.Generic;
    using System.Linq;
    using ESFA.DC.ILR.Model.Interface;
    using ESFA.DC.ILR.ValidationService.Data.Extensions;
    using ESFA.DC.ILR.ValidationService.Interface;
    using ESFA.DC.ILR.ValidationService.Rules.Abstract;
    using ESFA.DC.ILR.ValidationService.Rules.Constants;
    using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

    public class LearnFAMType_11Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnerFAMQueryService _learnerFAMQueryService;
        private readonly string[] _learnFamTypes =
        {
            LearnerFAMTypeConstants.NLM,
            LearnerFAMTypeConstants.EDF,
            LearnerFAMTypeConstants.PPE
        };

        public LearnFAMType_11Rule(ILearnerFAMQueryService learnerFamQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnFAMType_11)
        {
            _learnerFAMQueryService = learnerFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearnerFAMs == null)
            {
                return;
            }

            foreach (var learnerFam in objectToValidate.LearnerFAMs)
            {
                if (ConditionMet(learnerFam.LearnFAMType, objectToValidate.LearnerFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(learnerFam.LearnFAMType));
                }
            }
        }

        public bool ConditionMet(string learnerFamType, IEnumerable<ILearnerFAM> learnerFAMs)
        {
            return _learnFamTypes.Any(x => x.CaseInsensitiveEquals(learnerFamType)) &&
                   _learnerFAMQueryService.GetLearnerFAMsCountByFAMType(learnerFAMs, learnerFamType) > 2;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnFAMType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, learnFAMType)
            };
        }
    }
}
