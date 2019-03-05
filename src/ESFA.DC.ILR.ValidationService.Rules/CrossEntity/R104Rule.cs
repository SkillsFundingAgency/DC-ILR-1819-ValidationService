using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R104Rule : AbstractRule, IRule<ILearner>
    {
        private readonly string _famTypeACT = Monitoring.Delivery.Types.ApprenticeshipContract;

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public R104Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R104)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                var overlappingLearningDeliveryFAMs =
                    _learningDeliveryFAMQueryService
                    .GetOverLappingLearningDeliveryFAMsForType(learningDelivery.LearningDeliveryFAMs, _famTypeACT);

                foreach (var learningDeliveryFAM in overlappingLearningDeliveryFAMs)
                {
                    HandleValidationError(
                    objectToValidate.LearnRefNumber,
                    learningDelivery.AimSeqNumber,
                    errorMessageParameters: BuildErrorMessageParameters(
                        learningDelivery.LearnPlanEndDate,
                        learningDelivery.LearnActEndDateNullable,
                        _famTypeACT,
                        learningDeliveryFAM.LearnDelFAMDateFromNullable,
                        learningDeliveryFAM.LearnDelFAMDateToNullable));
                }
            }
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnPlanEndDate, DateTime? learnActEndDate, string famType, DateTime? learnDelFamDateFrom, DateTime? learnDelFamDateTo)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnPlanEndDate, learnPlanEndDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, famType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateFrom, learnDelFamDateFrom),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, learnDelFamDateTo)
            };
        }
    }
}
