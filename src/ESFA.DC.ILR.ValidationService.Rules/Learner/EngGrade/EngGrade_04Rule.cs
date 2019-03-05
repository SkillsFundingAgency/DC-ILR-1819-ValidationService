using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.EngGrade
{
    public class EngGrade_04Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnerFAMQueryService _learnerFamQueryService;

        public EngGrade_04Rule(
            ILearnerFAMQueryService learnerFamQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.EngGrade_04)
        {
            _learnerFamQueryService = learnerFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(
                objectToValidate.EngGrade,
                objectToValidate.LearnerFAMs))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.EngGrade));
            }
        }

        public bool ConditionMet(string engGrade, IEnumerable<ILearnerFAM> learnerFAMs)
        {
            return EngGradeConditionMet(engGrade)
                   && LearnerFAMConditionMet(learnerFAMs);
        }

        public bool EngGradeConditionMet(string engGrade)
        {
            return !engGrade.CaseInsensitiveEquals(ValidationConstants.None);
        }

        public bool LearnerFAMConditionMet(IEnumerable<ILearnerFAM> learnerFAMs)
        {
            var famCodes = new[] { 2, 3, 4 };

            return _learnerFamQueryService.HasAnyLearnerFAMCodesForType(learnerFAMs, Monitoring.Learner.Types.GCSEEnglishConditionOfFunding, famCodes);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string engGrade)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.EngGrade, engGrade),
            };
        }
    }
}
