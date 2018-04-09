using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.CompStatus.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.CompStatus
{
    public class CompStatus_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ICompStatusDataService _compStatusInternalDataService;

        public CompStatus_01Rule(ICompStatusDataService compStatusInternalDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
            _compStatusInternalDataService = compStatusInternalDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.CompStatus))
                {
                    HandleValidationError(RuleNameConstants.CompStatus_01, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber);
                }
            }
        }

        public bool ConditionMet(int compStatus)
        {
            return !_compStatusInternalDataService.Exists(compStatus);
        }
    }
}
