using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDHealthProb
{
    /// <summary>
    /// If the learner's LLDD and health problem is 'Learner does not consider himself or herself to have a learning difficulty and/or disability or health problem',
    /// then a LLDD and Health Problem record must not be returned
    /// </summary>
    public class LLDDHealthProb_06Rule : AbstractRule, IRule<ILearner>
    {
        private const int ValidLlddHealthProblemValue = 1;
        private readonly IDD06 _dd06;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public LLDDHealthProb_06Rule(IValidationErrorHandler validationErrorHandler, IDD06 dd06, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IDateTimeQueryService dateTimeQueryService)
            : base(validationErrorHandler)
        {
            _dd06 = dd06;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.LLDDHealthProbNullable, objectToValidate.LLDDAndHealthProblems) &&
                !Exclude(objectToValidate.LearningDeliveries, objectToValidate.DateOfBirthNullable))
            {
                HandleValidationError(RuleNameConstants.LLDDHealthProb_06Rule, objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(long? lldHealthProblem, IReadOnlyCollection<ILLDDAndHealthProblem> llddAndHealthProblems)
        {
            return lldHealthProblem.HasValue &&
                   lldHealthProblem.Value == ValidLlddHealthProblemValue &&
                   (llddAndHealthProblems == null || !llddAndHealthProblems.Any());
        }

        public bool Exclude(IReadOnlyCollection<ILearningDelivery> learningDeliveries, DateTime? dateOfBirth)
        {
            return (learningDeliveries != null && learningDeliveries.Any(x => ExcludeConditionFamValueMet(x.FundModelNullable, x.LearningDeliveryFAMs))) ||
                   ExcludeConditionDateOfBirthMet(dateOfBirth, _dd06.Derive(learningDeliveries));
        }

        public bool ExcludeConditionFamValueMet(long? fundModel, IReadOnlyCollection<ILearningDeliveryFAM> fams)
        {
            return fundModel.HasValue &&
                   (
                       fundModel.Value == 10 ||
                       (fundModel.Value == 99 && _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(fams, LearningDeliveryFAMTypeConstants.SOF, "108")));
        }

        public bool ExcludeConditionDateOfBirthMet(DateTime? dateOfBirth, DateTime? minimumLearningDeliveryStartDate)
        {
            return dateOfBirth.HasValue &&
                   minimumLearningDeliveryStartDate.HasValue &&
                   _dateTimeQueryService.YearsBetween(dateOfBirth.Value, minimumLearningDeliveryStartDate.Value) >= 25;
        }
    }
}