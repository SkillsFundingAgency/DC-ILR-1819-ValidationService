using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.InternalData.AimType.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AimType
{
    public class AimType_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IAimTypeInternalDataService _aimTypeInternalDataService;

        public AimType_01Rule(IAimTypeInternalDataService aimTypeInternalDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
            _aimTypeInternalDataService = aimTypeInternalDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.AimType))
                {
                    HandleValidationError(RuleNameConstants.AimType_01, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber);
                }
            }
        }

        public bool ConditionMet(int aimType)
        {
            return !_aimTypeInternalDataService.Exists(aimType);
        }
    }
}
