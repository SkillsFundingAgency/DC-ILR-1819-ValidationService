using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.InternalData.PriorAttain;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain
{
    public class PriorAttain_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IPriorAttainInternalDataService _priorAttainReferenceDataService;

        public PriorAttain_03Rule(IPriorAttainInternalDataService priorAttainReferenceDataService, IValidationErrorHandler validationErrorHandler)
           : base(validationErrorHandler)
        {
            _priorAttainReferenceDataService = priorAttainReferenceDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.PriorAttainNullable))
            {
                HandleValidationError(RuleNameConstants.PriorAttain_03Rule, objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(long? priorAttain)
        {
            return priorAttain.HasValue &&
                    !_priorAttainReferenceDataService.Exists(priorAttain.Value);
        }
    }
}