using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PartnerUKPRN
{
    public class PartnerUKPRN_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IOrganisationDataService _organisationDataService;

        public PartnerUKPRN_01Rule(IOrganisationDataService organisationDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PartnerUKPRN_01)
        {
            _organisationDataService = organisationDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.PartnerUKPRNNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters: BuildErrorMessageParameters(learningDelivery.PartnerUKPRNNullable));
                }
            }
        }

        public bool ConditionMet(long? partnerUKPRN)
        {
            return NullConditionMet(partnerUKPRN) && LookupConditionMet((long)partnerUKPRN);
        }

        public bool NullConditionMet(long? partnerUKPRN)
        {
            return partnerUKPRN.HasValue;
        }

        public bool LookupConditionMet(long partnerUKPRN)
        {
            return !_organisationDataService.IsPartnerUkprn(partnerUKPRN);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(long? partnerUKPRN)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PartnerUKPRN, partnerUKPRN)
            };
        }
    }
}
