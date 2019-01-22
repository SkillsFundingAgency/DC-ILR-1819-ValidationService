using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProvSpecDelMonOccur
{
    public class ProvSpecDelMonOccur_01Rule : AbstractRule, IRule<ILearner>
    {
        /// <summary>
        /// valid Provider specified learning delivery monitoring occurance values
        /// </summary>
        private readonly HashSet<string> validProvSpecDelMonOccurValues = new HashSet<string> { "A", "B", "C", "D" };

        public ProvSpecDelMonOccur_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ProvSpecDelMonOccur_01)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (learningDelivery.ProviderSpecDeliveryMonitorings == null)
                {
                    continue;
                }

                foreach (var providerSpecDeliveryMonitoring in learningDelivery.ProviderSpecDeliveryMonitorings)
                {
                    if (ConditionMet(providerSpecDeliveryMonitoring.ProvSpecDelMonOccur))
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(providerSpecDeliveryMonitoring.ProvSpecDelMonOccur));
                    }
                }
            }
        }

        public bool ConditionMet(string provSpecDelMonOccur)
        {
            return !string.IsNullOrWhiteSpace(provSpecDelMonOccur) && !validProvSpecDelMonOccurValues.Any(x => x.CaseInsensitiveEquals(provSpecDelMonOccur));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string provSpecDelMonOccur)
        {
            return new[]
            {
                BuildErrorMessageParameter("ProvSpecDelMonOccur", provSpecDelMonOccur)
            };
        }
    }
}
