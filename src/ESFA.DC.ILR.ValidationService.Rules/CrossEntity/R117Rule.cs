using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R117Rule : AbstractRule, IRule<ILearner>
    {
        private const string _famCode = "357";
        private const string _famType = LearningDeliveryFAMTypeConstants.LDM;
        private const int _aimType1 = 1;
        private const int _aimType3 = 3;
        private const int _progType24 = 24;

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public R117Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R117)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public R117Rule()
          : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            var componentAim = objectToValidate.LearningDeliveries?
                .Where(ld =>
                    ld.AimType == _aimType3
                && ld.ProgTypeNullable == _progType24)
                .FirstOrDefault();

            if (ConditionMet(componentAim, objectToValidate.LearningDeliveries))
            {
                HandleValidationError(
                    objectToValidate.LearnRefNumber,
                    componentAim.AimSeqNumber,
                    BuildErrorMessageParameters(
                        componentAim.AimType, componentAim.FundModel, componentAim.ProgTypeNullable, _famType, _famCode));
            }
        }

        public bool ConditionMet(ILearningDelivery componentAim, IEnumerable<ILearningDelivery> learningDeliveries)
        {
            if (componentAim != null)
            {
                return ComponentAimConditionMet(componentAim)
                    && ProgrammeAimConditionMet(componentAim.FundModel, componentAim.ProgTypeNullable, learningDeliveries);
            }

            return false;
        }

        public virtual bool ComponentAimConditionMet(ILearningDelivery componentAim)
        {
            return componentAim == null
                ? false
                : _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(componentAim.LearningDeliveryFAMs, _famType, _famCode);
        }

        public virtual bool ProgrammeAimConditionMet(int fundModel, int? progType, IEnumerable<ILearningDelivery> learningDeliveries)
        {
            var programmeAims = learningDeliveries?
                .Where(
                   ld => ld.AimType == _aimType1
                && ld.ProgTypeNullable == progType
                && ld.FundModel == fundModel);

            if (programmeAims.Any())
            {
                var learningDeliveryFams = programmeAims?
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
