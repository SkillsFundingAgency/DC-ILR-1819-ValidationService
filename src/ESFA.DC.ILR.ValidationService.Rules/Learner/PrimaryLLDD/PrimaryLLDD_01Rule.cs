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
    public class PrimaryLLDD_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _firstAugust2015 = new DateTime(2015, 08, 01);
        private readonly HashSet<long> _excludeLlddCatValues = new HashSet<long>() { 98, 99 };

        private readonly IDerivedData_06Rule _dd06;

        public PrimaryLLDD_01Rule(IDerivedData_06Rule dd06, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PrimaryLLDD_01)
        {
            _dd06 = dd06;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.LLDDHealthProb, objectToValidate.LLDDAndHealthProblems, objectToValidate.LearningDeliveries))
            {
                HandleValidationError(objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(int llddHealthProb, IEnumerable<ILLDDAndHealthProblem> llddAndHealthProblems, IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return LLDDHealthProbConditionMet(llddHealthProb)
                && LearnStartDateConditionMet(learningDeliveries)
                && LLDDConditionMet(llddAndHealthProblems);
        }

        public bool LLDDHealthProbConditionMet(int llddHealthProb)
        {
            return llddHealthProb == 1;
        }

        public bool LearnStartDateConditionMet(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return _dd06.Derive(learningDeliveries) >= _firstAugust2015;
        }

        public bool LLDDConditionMet(IEnumerable<ILLDDAndHealthProblem> llddAndHealthProblems)
        {
            return llddAndHealthProblems != null
                && !llddAndHealthProblems.All(x => _excludeLlddCatValues.Contains(x.LLDDCat))
                && !llddAndHealthProblems.Any(x => x.PrimaryLLDDNullable == 1);
        }
    }
}