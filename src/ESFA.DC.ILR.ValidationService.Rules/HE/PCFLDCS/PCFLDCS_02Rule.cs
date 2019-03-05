using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.PCFLDCS
{
    public class PCFLDCS_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILARSDataService _larsDataService;

        public PCFLDCS_02Rule(ILARSDataService larsDataService, IValidationErrorHandler validationErrorHandler)
        : base(validationErrorHandler, RuleNameConstants.PCFLDCS_02)
        {
            _larsDataService = larsDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearningDeliveryHEEntity, learningDelivery.LearnAimRef))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber);
                }
            }
        }

        public bool ConditionMet(ILearningDeliveryHE learningDeliveryHE, string learnAimRef)
        {
            return LearningDeliveryHEConditionMet(learningDeliveryHE)
                && LARSLearningDeliveryConditionMet(learnAimRef);
        }

        public bool LearningDeliveryHEConditionMet(ILearningDeliveryHE learningDeliveryHE)
        {
            return learningDeliveryHE != null
                && learningDeliveryHE.PCFLDCSNullable == null;
        }

        public bool LARSLearningDeliveryConditionMet(string learnAimRef)
        {
            return _larsDataService.LearnDirectClassSystemCode1MatchForLearnAimRef(learnAimRef);
        }
    }
}
