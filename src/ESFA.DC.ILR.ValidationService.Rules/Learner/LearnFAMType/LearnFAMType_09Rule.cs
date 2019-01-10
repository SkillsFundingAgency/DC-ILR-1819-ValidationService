using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType
{
    public class LearnFAMType_09Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnerFAMQueryService _learnerFAMQueryService;
        private readonly string[] _learnFamTypes =
        {
            LearnerFAMTypeConstants.HNS,
            LearnerFAMTypeConstants.EHC,
            LearnerFAMTypeConstants.DLA,
            LearnerFAMTypeConstants.SEN,
            LearnerFAMTypeConstants.MCF,
            LearnerFAMTypeConstants.ECF,
            LearnerFAMTypeConstants.FME
        };

        public LearnFAMType_09Rule(ILearnerFAMQueryService learnerFamQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnFAMType_09)
        {
            _learnerFAMQueryService = learnerFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
       {
            if (objectToValidate.LearnerFAMs == null)
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
            return _learnFamTypes.Contains(learnerFamType) &&
                   _learnerFAMQueryService.GetLearnerFAMsCountByFAMType(learnerFAMs, learnerFamType) > 1;
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
