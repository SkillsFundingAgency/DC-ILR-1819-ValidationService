using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PrimaryLLDD
{
    /// <summary>
    /// If the earliest Learning start date is on or after 1 August 2015, then the Primary LLDD and health problem must be recorded on one of the LLDD and health problem records
    /// </summary>
    public class PrimaryLLDD_01Rule : AbstractRule, IRule<ILearner>
    {
        private const int ValidPrimaryLldd = 1;
        private readonly DateTime _minimumLearningStartDateAllowed = new DateTime(2015, 08, 01);
        private readonly HashSet<long> _excludeLlddCatValues = new HashSet<long>() { 98, 99 };
        private readonly IDD06 _dd06;

        public PrimaryLLDD_01Rule(IValidationErrorHandler validationErrorHandler, IDD06 dd06)
            : base(validationErrorHandler)
        {
            _dd06 = dd06;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.LLDDAndHealthProblems, _dd06.Derive(objectToValidate.LearningDeliveries)) && !Exclude(objectToValidate.LLDDAndHealthProblems))
            {
                HandleValidationError(RuleNameConstants.PrimaryLLDD_01Rule, objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(IReadOnlyCollection<ILLDDAndHealthProblem> lLDDAndHealthProblems, DateTime? minimumStartDate)
        {
            return minimumStartDate.HasValue &&
                   minimumStartDate.Value >= _minimumLearningStartDateAllowed &&
                   ConditionMetAnyValidPrimaryLldd(lLDDAndHealthProblems);
        }

        public bool ConditionMetAnyValidPrimaryLldd(IReadOnlyCollection<ILLDDAndHealthProblem> lLDDAndHealthProblems)
        {
            return lLDDAndHealthProblems == null ||
                   !lLDDAndHealthProblems.Any(x => x.PrimaryLLDDNullable.HasValue && x.PrimaryLLDDNullable.Value == ValidPrimaryLldd);
        }

        public bool Exclude(IReadOnlyCollection<ILLDDAndHealthProblem> lLDDAndHealthProblems)
        {
            return lLDDAndHealthProblems != null &&
                   lLDDAndHealthProblems.All(x => x.LLDDCatNullable.HasValue && _excludeLlddCatValues.Contains(x.LLDDCatNullable.Value));
        }
    }
}