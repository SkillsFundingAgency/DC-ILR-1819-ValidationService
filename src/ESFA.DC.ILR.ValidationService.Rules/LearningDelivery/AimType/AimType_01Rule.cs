using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AimType.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AimType
{
    public class AimType_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IAimTypeDataService _aimTypeInternalDataService;

        public AimType_01Rule(IAimTypeDataService aimTypeInternalDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AimType_01)
        {
            _aimTypeInternalDataService = aimTypeInternalDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.AimType))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber);
                }
            }
        }

        public bool ConditionMet(int aimType)
        {
            return !_aimTypeInternalDataService.Exists(aimType);
        }
    }
}
