﻿using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ProvSpecLearnMonOccur
{
    /// <summary>
    /// If returned, Provider specified learner monitoring occurrence must be 'A' or 'B'
    /// </summary>
    public class ProvSpecLearnMonOccur_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<string> _validProvSpecLearnMonOccurValues = new HashSet<string>() { "A", "B" };

        public ProvSpecLearnMonOccur_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            // ILR Spec and ILR validation rules spreadsheet is suggesting that there will be only one value for ProviderSpecLearnerMonitoring, however xsd has it as list so treating it as a list
            if (objectToValidate.ProviderSpecLearnerMonitorings != null && objectToValidate.ProviderSpecLearnerMonitorings.Any())
            {
                foreach (var providerSpecLearnerMonitoring in objectToValidate.ProviderSpecLearnerMonitorings)
                {
                    if (ConditionMet(providerSpecLearnerMonitoring.ProvSpecLearnMonOccur))
                    {
                        HandleValidationError(RuleNameConstants.ProvSpecLearnMonOccur_01Rule, objectToValidate.LearnRefNumber);
                    }
                }
            }
        }

        public bool ConditionMet(string provSpecLearnMonOccur)
        {
            return !string.IsNullOrWhiteSpace(provSpecLearnMonOccur) && !_validProvSpecLearnMonOccurValues.Contains(provSpecLearnMonOccur);
        }
    }
}