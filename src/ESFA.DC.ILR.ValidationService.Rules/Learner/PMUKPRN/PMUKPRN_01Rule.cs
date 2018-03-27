using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.ExternalData.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PMUKPRN
{
    public class PMUKPRN_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IOrganisationReferenceDataService _organisationReferenceDataService;

        public PMUKPRN_01Rule(IOrganisationReferenceDataService organisationReferenceDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
            _organisationReferenceDataService = organisationReferenceDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (NullConditionMet(objectToValidate.PMUKPRNNullable)
                && LookupConditionMet(_organisationReferenceDataService.UkprnExists(objectToValidate.PMUKPRNNullable.Value)))
            {
                HandleValidationError(RuleNameConstants.PMUKPRN_01, objectToValidate.LearnRefNumber);
            }
        }

        public bool NullConditionMet(long? pmUkprn)
        {
            return pmUkprn.HasValue;
        }

        public bool LookupConditionMet(bool ukprnExists)
        {
            return !ukprnExists;
        }
    }
}