using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILARSDataService _larsDataService;

        public LearnAimRef_01Rule(ILARSDataService larsDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_01)
        {
            _larsDataService = larsDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearnAimRef))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnAimRef));
                }
            }
        }

        public bool ConditionMet(string learnAimRef)
        {
            return !_larsDataService.LearnAimRefExists(learnAimRef);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnAimRef)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef)
            };
        }
    }
}
