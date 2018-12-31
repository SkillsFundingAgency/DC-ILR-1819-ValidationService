using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.EngGrade
{
    public class EngGrade_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<string> _learnAimRefTypes = new HashSet<string>() { "0003", "1422", "2999", "NONE" };
        private readonly ILARSDataService _lARSDataService;

        public EngGrade_02Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService lARSDataService)
            : base(validationErrorHandler, RuleNameConstants.EngGrade_02)
        {
            _lARSDataService = lARSDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null
                && !EngGradeConditionMet(objectToValidate?.EngGrade))
            {
                return;
            }

            foreach (var learninDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learninDelivery.LearnAimRef))
                {
                    HandleValidationError(
                        learnRefNumber: objectToValidate.LearnRefNumber,
                        aimSequenceNumber: learninDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(objectToValidate.EngGrade));
                }
            }
        }

        public bool ConditionMet(string learnAimRef)
        {
            return !_lARSDataService.HasAnyLearningDeliveryForLearnAimRefAndTypes(learnAimRef, _learnAimRefTypes);
        }

        public bool EngGradeConditionMet(string engGrade)
        {
            return !string.IsNullOrEmpty(engGrade);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string engGrade)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.EngGrade, engGrade)
            };
        }
    }
}
