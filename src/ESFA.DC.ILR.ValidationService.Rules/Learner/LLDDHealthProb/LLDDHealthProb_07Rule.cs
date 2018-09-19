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
    /// If the LLDD and health problem is 'Learner considers himself or herself to have a learning difficulty and/or disability or health problem',
    /// then a LLDD and Health Problem record must be returned if the Planned learning hours > 10
    /// </summary>
    public class LLDDHealthProb_07Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int _validLLDDHealthProblemValue = 1;
        private readonly IDD06 _dd06;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public LLDDHealthProb_07Rule(IValidationErrorHandler validationErrorHandler, IDD06 dd06, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IDateTimeQueryService dateTimeQueryService)
            : base(validationErrorHandler)
        {
            _dd06 = dd06;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(
                    objectToValidate.LLDDHealthProbNullable,
                    objectToValidate.PlanLearnHoursNullable,
                    objectToValidate.LLDDAndHealthProblems,
                    objectToValidate.LearningDeliveries) &&
                !Exclude(objectToValidate.DateOfBirthNullable, _dd06.Derive(objectToValidate.LearningDeliveries)))
            {
                HandleValidationError(RuleNameConstants.LLDDHealthProb_07Rule, objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(long? lldHealthProblem, long? planLearnHours, IReadOnlyCollection<ILLDDAndHealthProblem> llddAndHealthProblems, IReadOnlyCollection<ILearningDelivery> learningDeliveries)
        {
            return LearningDeliveriesConditionMet(learningDeliveries) &&
                   ConditionLLDHealthConditionMet(lldHealthProblem) &&
                   ConditionLLDDHealthAndProblemsMet(llddAndHealthProblems) &&
                   ConditionPlannedLearnHoursMet(planLearnHours);
        }

        public bool LearningDeliveriesConditionMet(IReadOnlyCollection<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries != null &&
                   learningDeliveries.All(x =>
                       ConditionFamValueMet(x.FundModelNullable, x.LearningDeliveryFAMs));
        }

        public bool ConditionLLDHealthConditionMet(long? lldHealthProblem)
        {
            return lldHealthProblem.HasValue && lldHealthProblem.Value == _validLLDDHealthProblemValue;
        }

        public bool ConditionFamValueMet(long? fundModel, IReadOnlyCollection<ILearningDeliveryFAM> fams)
        {
            return fundModel.HasValue &&
                   (
                       fundModel.Value == 10 ||
                       (fundModel.Value == 99 && _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(fams, LearningDeliveryFAMTypeConstants.SOF, "108")));
        }

        public bool ConditionPlannedLearnHoursMet(long? planLearnHours)
        {
            return planLearnHours.HasValue && planLearnHours.Value > 10;
        }

        public bool ConditionLLDDHealthAndProblemsMet(IReadOnlyCollection<ILLDDAndHealthProblem> llddAndHealthProblems)
        {
            return llddAndHealthProblems == null || !llddAndHealthProblems.Any();
        }

        public bool Exclude(DateTime? dateOfBirth, DateTime? minimumLearningDeliveryStartDate)
        {
            return dateOfBirth.HasValue &&
                   minimumLearningDeliveryStartDate.HasValue &&
                   _dateTimeQueryService.YearsBetween(dateOfBirth.Value, minimumLearningDeliveryStartDate.Value) >= 25;
        }
    }
}