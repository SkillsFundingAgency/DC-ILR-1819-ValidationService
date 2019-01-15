using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
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
        /// <summary>
        /// valid Provider specific Learner Monitoring Occurence Values
        /// </summary>
        private static readonly HashSet<string> ValidProvSpecLearnMonOccurValues = new HashSet<string> { "A", "B" };

        public ProvSpecLearnMonOccur_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ProvSpecLearnMonOccur_01Rule)
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
                        HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(providerSpecLearnerMonitoring.ProvSpecLearnMonOccur));
                    }
                }
            }
        }

        public bool ConditionMet(string provSpecLearnMonOccur)
        {
            return !string.IsNullOrWhiteSpace(provSpecLearnMonOccur) && !ValidProvSpecLearnMonOccurValues.Any(x => x.CaseInsensitiveEquals(provSpecLearnMonOccur));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string provSpecLearnMonOccur)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ProvSpecLearnMonOccur, provSpecLearnMonOccur)
            };
        }
    }
}