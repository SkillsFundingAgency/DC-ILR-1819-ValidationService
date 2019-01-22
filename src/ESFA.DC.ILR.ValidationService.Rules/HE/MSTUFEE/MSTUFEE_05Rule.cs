using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.MSTUFEE
{
    public class MSTUFEE_05Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int[] _mstufeeCodes = { 3, 4 };
        private readonly string[] _domicileCodes = { "XF", "XI" };

        public MSTUFEE_05Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.MSTUFEE_05)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(x => x.LearningDeliveryHEEntity != null))
            {
                if (ConditionMet(learningDelivery.LearningDeliveryHEEntity))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learningDelivery.LearningDeliveryHEEntity.MSTUFEE,
                            learningDelivery.LearningDeliveryHEEntity.DOMICILE));
                }
            }
        }

        public bool ConditionMet(ILearningDeliveryHE learningDeliveryHe)
        {
            return learningDeliveryHe != null
                   && _mstufeeCodes.Contains(learningDeliveryHe.MSTUFEE)
                   && _domicileCodes.Any(x => x.CaseInsensitiveEquals(learningDeliveryHe.DOMICILE));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int mstufee, string domicile)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.MSTUFEE, mstufee),
                BuildErrorMessageParameter(PropertyNameConstants.DOMICILE, domicile)
            };
        }
    }
}
