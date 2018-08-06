using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.InternalData.LearnFAMTypeCode;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType
{
    /// <summary>
    /// If a FAM type is returned, the FAM code must be a valid lookup for that FAM type
    /// </summary>
    public class LearnFAMType_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnFAMTypeCodeInternalDataService _learnFAMTypeCodeInternalDataService;

        public LearnFAMType_01Rule(IValidationErrorHandler validationErrorHandler, ILearnFAMTypeCodeInternalDataService learnFAMTypeCodeInternalDataService)
            : base(validationErrorHandler)
        {
            _learnFAMTypeCodeInternalDataService = learnFAMTypeCodeInternalDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learnerFam in objectToValidate.LearnerFAMs)
            {
                if (ConditionMet(learnerFam.LearnFAMType, learnerFam.LearnFAMCodeNullable))
                {
                    HandleValidationError(RuleNameConstants.LearnFAMType_01Rule, objectToValidate.LearnRefNumber);
                }
            }
        }

        public bool ConditionMet(string famType, long? famCode)
        {
            return !string.IsNullOrWhiteSpace(famType) &&
                   famCode.HasValue &&
                  !_learnFAMTypeCodeInternalDataService.TypeCodeExists(famType, famCode);
        }
    }
}