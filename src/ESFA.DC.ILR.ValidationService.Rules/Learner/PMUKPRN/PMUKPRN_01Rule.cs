using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PMUKPRN
{
    public class PMUKPRN_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IOrganisationDataService _organisationDataService;

        public PMUKPRN_01Rule(IOrganisationDataService organisationDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PMUKPRN_01)
        {
            _organisationDataService = organisationDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.PMUKPRNNullable))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.PMUKPRNNullable));
            }
        }

        public bool ConditionMet(long? pmUkprn)
        {
            return NullConditionMet(pmUkprn) && LookupConditionMet((long)pmUkprn);
        }

        public bool NullConditionMet(long? pmUkprn)
        {
            return pmUkprn.HasValue;
        }

        public bool LookupConditionMet(long pmUkprn)
        {
            return !_organisationDataService.IsPartnerUkprn(pmUkprn);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(long? pmUKPRN)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PMUKPRN, pmUKPRN)
            };
        }
    }
}