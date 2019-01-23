﻿using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.CampId
{
    public class CampId_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IOrganisationDataService _organisationDataService;

        public CampId_02Rule(IOrganisationDataService organisationDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.CampId_02)
        {
            _organisationDataService = organisationDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(objectToValidate.CampId))
            {
                if (ConditionMet(objectToValidate.CampId))
                {
                    HandleValidationError(learnRefNumber: objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.CampId));
                }
            }
        }

        public bool ConditionMet(string campId)
        {
            return true;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string campId)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.CampId, campId)
            };
        }
    }
}