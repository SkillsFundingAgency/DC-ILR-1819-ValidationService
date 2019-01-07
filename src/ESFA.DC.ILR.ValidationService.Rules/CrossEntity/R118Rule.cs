using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R118Rule : AbstractRule, IRule<ILearner>
    {
        private readonly string _famCode = "357";
        private readonly string _famType = LearningDeliveryFAMTypeConstants.LDM;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public R118Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R118)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            var learningDeliveryAimType1 = objectToValidate.LearningDeliveries?.Where(ld => ld.AimType == 1 && ld.ProgTypeNullable == 24).FirstOrDefault();

            if (ConditionMet(learningDeliveryAimType1, objectToValidate.LearningDeliveries))
            {
                HandleValidationError(
                    objectToValidate.LearnRefNumber,
                    learningDeliveryAimType1.AimSeqNumber,
                    BuildErrorMessageParameters(
                        learningDeliveryAimType1.AimType, learningDeliveryAimType1.FundModel, learningDeliveryAimType1.ProgTypeNullable, _famType, _famCode));
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery, IEnumerable<ILearningDelivery> learningDeliveries)
        {
            if (learningDelivery != null)
            {
                return ComponentAimConditionMet(learningDelivery.FundModel, learningDelivery.ProgTypeNullable, learningDeliveries);
            }

            return false;
        }

        public bool ComponentAimConditionMet(int fundModel, int? progType, IEnumerable<ILearningDelivery> learningDeliveries)
        {
            var componentAims = learningDeliveries?
                .Where(
                   ld => ld.AimType == 3
                && ld.ProgTypeNullable == progType
                && ld.FundModel == fundModel)
                .Select(ld => ld);

            if (componentAims.Any())
            {
                var learningDeliveryFams = componentAims?
                    .Where(ld => ld.LearningDeliveryFAMs != null)
                    .SelectMany(ld => ld.LearningDeliveryFAMs);

                return !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, _famType, _famCode);
            }

            return true;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int aimType, int fundModel, int? progType, string famType, string famCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, famType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, famCode)
            };
        }
    }
}
