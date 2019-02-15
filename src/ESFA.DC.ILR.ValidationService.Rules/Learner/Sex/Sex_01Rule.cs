using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.Sex
{
    /// <summary>
    /// The learner's Sex must be a valid lookup
    /// </summary>
    public class Sex_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        public Sex_01Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideLookupDetails provideLookupDetails)
            : base(validationErrorHandler, RuleNameConstants.Sex_01)
        {
            _provideLookupDetails = provideLookupDetails;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate == null)
            {
                return;
            }

            if (ConditionMet(objectToValidate.Sex))
            {
                HandleValidationError(
                    objectToValidate.LearnRefNumber,
                    errorMessageParameters: BuildErrorMessageParameters(objectToValidate.Sex));
            }
        }

        public bool ConditionMet(string sex) => !_provideLookupDetails.Contains(TypeOfStringCodedLookup.Sex, sex);

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string sex)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.Sex, sex)
            };
        }
    }
}