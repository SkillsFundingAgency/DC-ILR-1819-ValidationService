using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_06Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _lookupDetails;

        public LearnDelFAMType_06Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideLookupDetails lookupDetails)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_06)
        {
            _lookupDetails = lookupDetails;
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (learningDelivery.LearningDeliveryFAMs == null)
                {
                    continue;
                }

                if (learningDelivery.LearningDeliveryFAMs.Any(ldf => ldf.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.RES)))
                {
                    continue;
                }

                foreach (var learningDeliveryFam in learningDelivery.LearningDeliveryFAMs)
                {
                    if (!IsValid(learningDelivery, learningDeliveryFam))
                    {
                        RaiseValidationMessage(objectToValidate.LearnRefNumber, learningDeliveryFam);
                    }
                }
            }
        }

        private bool IsValid(ILearningDelivery learningDelivery, ILearningDeliveryFAM monitor)
        {
            return _lookupDetails.IsCurrent(
                LookupComplexKey.LearningDeliveryFAM,
                monitor.LearnDelFAMType,
                monitor.LearnDelFAMCode,
                learningDelivery.LearnStartDate);
        }

        private void RaiseValidationMessage(string learnRefNumber, ILearningDeliveryFAM thisMonitor)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                BuildErrorMessageParameter(nameof(thisMonitor.LearnDelFAMType), thisMonitor.LearnDelFAMType),
                BuildErrorMessageParameter(nameof(thisMonitor.LearnDelFAMCode), thisMonitor.LearnDelFAMCode)
            };

            HandleValidationError(learnRefNumber, null, parameters);
        }
    }
}
