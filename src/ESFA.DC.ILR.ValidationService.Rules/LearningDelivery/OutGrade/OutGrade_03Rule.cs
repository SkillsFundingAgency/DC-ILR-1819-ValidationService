using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OutGrade
{
    public class OutGrade_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<string> _outGrades = new HashSet<string> { "EL1", "EL2", "EL3" };

        private readonly ILARSDataService _larsDataService;

        public OutGrade_03Rule(ILARSDataService larsDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.OutGrade_03)
        {
            _larsDataService = larsDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.OutcomeNullable, learningDelivery.OutGrade, learningDelivery.LearnAimRef))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters: BuildErrorMessageParameters(learningDelivery.OutGrade));
                }
            }
        }

        public bool ConditionMet(int? outcome, string outGrade, string learnAimRef)
        {
            return OutcomeCondtionMet(outcome)
                && OutGradeCondtionMet(outGrade)
                && LARSConditionMet(learnAimRef);
        }

        public bool OutcomeCondtionMet(int? outcome)
        {
            return outcome != null && outcome == 1;
        }

        public bool OutGradeCondtionMet(string outGrade)
        {
            return !_outGrades.Contains(outGrade);
        }

        public bool LARSConditionMet(string learnAimRef)
        {
            return _larsDataService.BasicSkillsMatchForLearnAimRef(learnAimRef, 1);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string outGrade)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.OutGrade, outGrade)
            };
        }
    }
}
