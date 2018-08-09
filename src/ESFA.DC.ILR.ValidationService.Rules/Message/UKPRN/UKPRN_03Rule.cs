using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Message.UKPRN
{
    public class UKPRN_03Rule : AbstractRule, IRule<IMessage>
    {
        public UKPRN_03Rule(IValidationErrorHandler validationErrorHandler)
          : base(validationErrorHandler, RuleNameConstants.UKPRN_03)
        {
        }

        public void Validate(IMessage objectToValidate)
        {
            var sourceUKPRN = objectToValidate.HeaderEntity.SourceEntity.UKPRN;
            var learningProviderUKPRN = objectToValidate.LearningProviderEntity.UKPRN;

            if (ConditionMet(sourceUKPRN, learningProviderUKPRN))
            {
                HandleValidationError(errorMessageParameters: BuildErrorMessageParameters(learningProviderUKPRN));
                return;
            }
        }

        public bool ConditionMet(int sourceUKPRN, int learningProviderUKPRN)
        {
            return sourceUKPRN != learningProviderUKPRN;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int ukprn)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.UKPRN, ukprn)
            };
        }
    }
}
