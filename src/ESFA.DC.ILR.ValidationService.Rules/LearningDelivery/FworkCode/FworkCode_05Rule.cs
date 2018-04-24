using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FworkCode
{
    public class FworkCode_05Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDD07 _dd07;
        private readonly ILARSDataService _larsDataService;

        public FworkCode_05Rule(IDD07 dd07, ILARSDataService larsDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.FworkCode_05)
        {
            _dd07 = dd07;
            _larsDataService = larsDataService;
        }

        public FworkCode_05Rule()
            : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.AimType,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.LearnAimRef,
                    learningDelivery.FworkCodeNullable,
                    learningDelivery.PwayCodeNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.ProgTypeNullable, learningDelivery.FworkCodeNullable, learningDelivery.PwayCodeNullable));
                }
            }
        }

        public bool ConditionMet(int aimType, int? progType, string learnAimRef, int? fworkCode, int? pwayCode)
        {
            return AimTypeConditionMet(aimType)
                   && ApprenticeshipConditionMet(progType)
                   && FworkCodeConditionMet(learnAimRef, progType, fworkCode, pwayCode);
        }

        public virtual bool AimTypeConditionMet(int aimType)
        {
            return aimType == 3;
        }

        public virtual bool ApprenticeshipConditionMet(int? progType)
        {
            return progType != 25 && _dd07.Derive(progType) == ValidationConstants.Y;
        }

        public virtual bool FworkCodeConditionMet(string learnAimRef, int? progType, int? fworkCode, int? pwayCode)
        {
            return !_larsDataService.FrameworkCodeExistsForFrameworkAims(learnAimRef, progType, fworkCode, pwayCode)
                && !_larsDataService.FrameworkCodeExistsForCommonComponent(learnAimRef, progType, fworkCode, pwayCode);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? progType, int? fworkCode, int? pwayCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
                BuildErrorMessageParameter(PropertyNameConstants.FworkCode, fworkCode),
                BuildErrorMessageParameter(PropertyNameConstants.PwayCode, pwayCode),
            };
        }
    }
}
