using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.TYPEYR
{
    public class TYPEYR_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _provideLookupDetails;

        public TYPEYR_01Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideLookupDetails provideLookupDetails)
            : base(validationErrorHandler, RuleNameConstants.TYPEYR_01)
        {
            _provideLookupDetails = provideLookupDetails;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (learningDelivery.LearningDeliveryHEEntity != null)
                {
                    if (ConditionMet(learningDelivery.LearningDeliveryHEEntity.TYPEYR.ToString()))
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(learningDelivery.LearningDeliveryHEEntity.TYPEYR));
                    }
                }
            }
        }

        public bool ConditionMet(string typeYr) => !_provideLookupDetails.Contains(TypeOfStringCodedLookup.TypeYr, typeYr);

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int typeYr)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.TYPEYR, typeYr)
            };
        }
    }
}
