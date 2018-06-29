using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PrevUKPRN
{
    public class PrevUKPRN_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IOrganisationDataService _organisationDataService;

        public PrevUKPRN_01Rule(IOrganisationDataService organisationDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PrevUKPRN_01)
        {
            _organisationDataService = organisationDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (ConditionMet(objectToValidate.PrevUKPRNNullable))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.PrevUKPRNNullable));
            }
        }

        public bool ConditionMet(long? prevUkprn)
        {
            return NullConditionMet(prevUkprn) && LookupConditionMet((long)prevUkprn);
        }

        public bool NullConditionMet(long? prevUkprn)
        {
            return prevUkprn.HasValue;
        }

        public bool LookupConditionMet(long prevUkprn)
        {
            return !_organisationDataService.IsPartnerUkprn(prevUkprn);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(long? prevUKPRN)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PrevUKPRN, prevUKPRN)
            };
        }
    }
}