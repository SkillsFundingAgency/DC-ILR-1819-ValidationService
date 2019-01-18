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
    public class R101Rule : AbstractRule, IRule<ILearner>
    {
        private readonly string _famTypeACT = Monitoring.Delivery.Types.ApprenticeshipContract;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public R101Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R101)
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
                var learningDeliveryFAMs = _learningDeliveryFAMQueryService
                  .GetLearningDeliveryFAMsForType(learningDelivery.LearningDeliveryFAMs, _famTypeACT).ToList();

                if (DoesNotHaveMultipleACTFams(learningDeliveryFAMs))
                {
                    return;
                }

                var invalidLearningDeliveryFAMs = LearningDeliveryFamForOverlappingACTTypes(learningDeliveryFAMs);

                if (invalidLearningDeliveryFAMs.Count() > 0)
                {
                    foreach (var learningDeliveryFAM in invalidLearningDeliveryFAMs)
                    {
                        HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(
                            _famTypeACT,
                            learningDeliveryFAM.LearnDelFAMDateFromNullable,
                            learningDeliveryFAM.LearnDelFAMDateToNullable));
                    }
                }
            }
        }

        public bool DoesNotHaveMultipleACTFams(IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.GetLearningDeliveryFAMsCountByFAMType(learningDeliveryFAMs, _famTypeACT) < 2;
        }

        public IEnumerable<ILearningDeliveryFAM> LearningDeliveryFamForOverlappingACTTypes(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            var invalidLearningDeliveryFAMs = new List<ILearningDeliveryFAM>();

            if (learningDeliveryFAMs != null)
            {
                var ldFAMs = learningDeliveryFAMs.OrderBy(ld => ld.LearnDelFAMDateFromNullable).ToArray();

                var ldFAMsCount = ldFAMs.Length;

                var i = 1;

                while (i < ldFAMsCount)
                {
                    if (ldFAMs[i - 1].LearnDelFAMDateToNullable == null)
                    {
                        invalidLearningDeliveryFAMs.Add(ldFAMs[i - 1]);
                        i++;

                        continue;
                    }

                    var errorConditionMet =
                        ldFAMs[i].LearnDelFAMDateFromNullable == null
                        ? false
                        : ldFAMs[i - 1].LearnDelFAMDateToNullable >= ldFAMs[i].LearnDelFAMDateFromNullable;

                    if (errorConditionMet)
                    {
                        invalidLearningDeliveryFAMs.Add(ldFAMs[i - 1]);
                        i++;

                        continue;
                    }

                    i++;
                }
            }

            return invalidLearningDeliveryFAMs;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string famType, DateTime? famDateFrom, DateTime? famDateTo)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, famType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateFrom, famDateFrom),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, famDateTo)
            };
        }
    }
}
