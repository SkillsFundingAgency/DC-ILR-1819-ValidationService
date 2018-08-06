using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PartnerUKPRN
{
    public class PartnerUKPRN_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFileDataCache _fileDataCache;

        public PartnerUKPRN_03Rule(IFileDataCache fileDataCache, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PartnerUKPRN_03)
        {
            _fileDataCache = fileDataCache;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.PartnerUKPRNNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters: BuildErrorMessageParameters(_fileDataCache.UKPRN, learningDelivery.PartnerUKPRNNullable));
                }
            }
        }

        public bool ConditionMet(long? partnerUKPRN)
        {
            return NullConditionMet(partnerUKPRN) && UKPRNConditionMet((long)partnerUKPRN);
        }

        public bool NullConditionMet(long? partnerUKPRN)
        {
            return partnerUKPRN.HasValue;
        }

        public bool UKPRNConditionMet(long partnerUKPRN)
        {
            return _fileDataCache.UKPRN == partnerUKPRN;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(long learningProiderUKPRN, long? partnerUKPRN)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.UKPRN, learningProiderUKPRN),
                BuildErrorMessageParameter(PropertyNameConstants.PartnerUKPRN, partnerUKPRN)
            };
        }
    }
}
