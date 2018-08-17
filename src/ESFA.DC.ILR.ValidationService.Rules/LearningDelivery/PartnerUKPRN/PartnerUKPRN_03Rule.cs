using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PartnerUKPRN
{
    public class PartnerUKPRN_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFileDataService _fileDataService;

        public PartnerUKPRN_03Rule(IFileDataService fileDatService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PartnerUKPRN_03)
        {
            _fileDataService = fileDatService;
        }

        public void Validate(ILearner objectToValidate)
        {
            var ukprn = _fileDataService.UKPRN();

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(ukprn, learningDelivery.PartnerUKPRNNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters: BuildErrorMessageParameters(ukprn, learningDelivery.PartnerUKPRNNullable));
                }
            }
        }

        public bool ConditionMet(int ukprn, long? partnerUKPRN)
        {
            return NullConditionMet(partnerUKPRN) && UKPRNConditionMet(ukprn, (long)partnerUKPRN);
        }

        public bool NullConditionMet(long? partnerUKPRN)
        {
            return partnerUKPRN.HasValue;
        }

        public bool UKPRNConditionMet(int ukprn, long partnerUKPRN)
        {
            return ukprn == partnerUKPRN;
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
