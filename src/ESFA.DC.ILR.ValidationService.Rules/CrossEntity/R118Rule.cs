using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R118Rule : AbstractRule, IRule<ILearner>
    {
        private const string _famCode = "357";
        private const string _famType = LearningDeliveryFAMTypeConstants.LDM;
        private const int _aimType1 = 1;
        private const int _aimType3 = 3;
        private const int _progType24 = 24;

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public R118Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R118)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public R118Rule()
          : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            var programmeAim = objectToValidate.LearningDeliveries?
                .Where(ld =>
                    ld.AimType == _aimType1
                && ld.ProgTypeNullable == _progType24)
                .FirstOrDefault();

            if (ConditionMet(programmeAim, objectToValidate.LearningDeliveries))
            {
                HandleValidationError(
                    objectToValidate.LearnRefNumber,
                    programmeAim.AimSeqNumber,
                    BuildErrorMessageParameters(
                        programmeAim.AimType, programmeAim.FundModel, programmeAim.ProgTypeNullable, _famType, _famCode));
            }
        }

        public bool ConditionMet(ILearningDelivery programmeAim, IEnumerable<ILearningDelivery> learningDeliveries)
        {
            if (programmeAim != null)
            {
                return ProgrammeAimConditionMet(programmeAim)
                    && ComponentAimConditionMet(programmeAim.FundModel, programmeAim.ProgTypeNullable, learningDeliveries);
            }

            return false;
        }

        public virtual bool ProgrammeAimConditionMet(ILearningDelivery programmeAim)
        {
            return programmeAim == null
                ? false
                : _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(programmeAim.LearningDeliveryFAMs, _famType, _famCode);
        }

        public virtual bool ComponentAimConditionMet(int fundModel, int? progType, IEnumerable<ILearningDelivery> learningDeliveries)
        {
            var componentAims = learningDeliveries?
                .Where(
                   ld => ld.AimType == _aimType3
                && ld.ProgTypeNullable == progType
                && ld.FundModel == fundModel);

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
