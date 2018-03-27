using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.InternalData.LearnFAMTypeCode;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType
{
    /// <summary>
    /// The earliest Learning start date must not be after the 'Valid to' date for this FAMType and FAMCode
    /// </summary>
    public class LearnFAMType_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnFAMTypeCodeInternalDataService _learnFAMTypeCodeInternalDataService;
        private readonly IDD06 _dd06;

        public LearnFAMType_03Rule(IValidationErrorHandler validationErrorHandler, ILearnFAMTypeCodeInternalDataService learnFAMTypeCodeInternalDataService, IDD06 dd06)
            : base(validationErrorHandler)
        {
            _learnFAMTypeCodeInternalDataService = learnFAMTypeCodeInternalDataService;
            _dd06 = dd06;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learnerFam in objectToValidate.LearnerFAMs)
            {
                if (ConditionMet(
                    learnerFam.LearnFAMType,
                    learnerFam.LearnFAMCodeNullable,
                    _dd06.Derive(objectToValidate.LearningDeliveries)))
                {
                    HandleValidationError(RuleNameConstants.LearnFAMType_03Rule, objectToValidate.LearnRefNumber);
                }
            }
        }

        public bool ConditionMet(string famType, long? famCode, DateTime? minimumLearnStartDateTime)
        {
            return !string.IsNullOrWhiteSpace(famType) &&
                   famCode.HasValue &&
                   minimumLearnStartDateTime.HasValue &&
                  !_learnFAMTypeCodeInternalDataService.TypeCodeForDateExists(famType, famCode, minimumLearnStartDateTime);
        }
    }
}