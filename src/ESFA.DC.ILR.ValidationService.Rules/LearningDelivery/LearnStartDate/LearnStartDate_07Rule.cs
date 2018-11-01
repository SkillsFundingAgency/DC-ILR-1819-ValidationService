using System;
using System.Collections.Generic;
using System.Globalization;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_07Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDD07 _dd07;
        private readonly IDD04 _dd04;
        private readonly ILARSDataService _larsDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public LearnStartDate_07Rule(
            IDD07 dd07,
            IDD04 dd04,
            ILARSDataService larsDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnStartDate_07)
        {
            _dd07 = dd07;
            _dd04 = dd04;
            _larsDataService = larsDataService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                DateTime? dd04Date = _dd04.Derive(objectToValidate.LearningDeliveries, learningDelivery);

                if (ConditionMet(
                    learningDelivery.AimType,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.LearnAimRef,
                    dd04Date,
                    learningDelivery.FworkCodeNullable,
                    learningDelivery.PwayCodeNullable,
                    learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(learningDelivery.LearnStartDate, learningDelivery.PwayCodeNullable, learningDelivery.ProgTypeNullable, learningDelivery.FworkCodeNullable));
                }
            }
        }

        public bool ConditionMet(int aimType, int? progType, string learnAimRef, DateTime? dd04Date, int? fworkCode, int? pwayCode, IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return DD07ConditionMet(progType)
                   && AimTypeConditionMet(aimType)
                   && FrameworkAimsConditionMet(dd04Date, learnAimRef, progType, fworkCode, pwayCode)
                   && !Excluded(progType, learningDeliveryFams);
        }

        public bool DD07ConditionMet(int? progType)
        {
            return _dd07.IsApprenticeship(progType);
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == 3;
        }

        public bool FrameworkAimsConditionMet(DateTime? dd04Date, string learnAimRef, int? progType, int? fworkCode, int? pwayCode)
        {
            return _larsDataService.DD04DateGreaterThanFrameworkAimEffectiveTo(dd04Date, learnAimRef, progType, fworkCode, pwayCode);
        }

        public bool Excluded(int? progType, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return progType == 25
                   || _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES");
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, int? pwayCode, int? progType, int? fWorkCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate.ToString("d", new CultureInfo("en-GB"))),
                BuildErrorMessageParameter(PropertyNameConstants.PwayCode, pwayCode),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
                BuildErrorMessageParameter(PropertyNameConstants.FworkCode, fWorkCode),
            };
        }
    }
}
