using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.NiNumber
{
    /// <summary>
    /// If the learner is on an apprenticeship funded through a contract for services with the employer, then the NI Number must be returned
    /// (LearningDelivery.LearnDelFAMType = ACT and LearningDelivery.LearnDelFAMCode = 1) and Learner.NINumber is null
    /// </summary>
    public class NINumber_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;

        public NINumber_02Rule(IValidationErrorHandler validationErrorHandler, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService)
           : base(validationErrorHandler)
        {
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    objectToValidate.NINumber,
                    _learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT, "1")))
                {
                    HandleValidationError(RuleNameConstants.NINumber_02, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
                }
            }
        }

        public bool ConditionMet(string niNumber, bool hasApplicableFam)
        {
            return string.IsNullOrWhiteSpace(niNumber)
                && hasApplicableFam;
        }
    }
}