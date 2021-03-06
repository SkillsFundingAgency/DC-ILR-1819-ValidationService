﻿using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R112Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public R112Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.R112)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner learner)
        {
            if (learner.LearningDeliveries == null)
            {
                return;
            }

            ILearningDelivery ldToCheck = GetLearningDeliveryWithLatestFAMDateFrom(learner.LearningDeliveries);

            if (ldToCheck == null)
            {
                return;
            }

            if (ldToCheck.LearnActEndDateNullable.HasValue &&
                _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(ldToCheck.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT))
            {
                foreach (var ldfam in ldToCheck.LearningDeliveryFAMs)
                {
                    if (ldfam.LearnDelFAMType == LearningDeliveryFAMTypeConstants.ACT &&
                        (!ldfam.LearnDelFAMDateToNullable.HasValue ||
                        ldfam.LearnDelFAMDateToNullable.Value != ldToCheck.LearnActEndDateNullable))
                    {
                        HandleValidationError(
                            learner.LearnRefNumber,
                            ldToCheck.AimSeqNumber,
                            BuildErrorMessageParameters(ldToCheck.LearnActEndDateNullable, LearningDeliveryFAMTypeConstants.ACT, ldfam.LearnDelFAMDateToNullable));
                    }
                }
            }
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? learnActEndDateNullable, string learnDelFAMType, DateTime? learnDelFAMDateTo)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDateNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, learnDelFAMDateTo)
            };
        }

        private ILearningDelivery GetLearningDeliveryWithLatestFAMDateFrom(IReadOnlyCollection<ILearningDelivery> learningDeliveries)
        {
            DateTime latestACTFAMFrom = DateTime.MinValue;
            ILearningDelivery result = null;
            foreach (var learningDelivery in learningDeliveries)
            {
                if (learningDelivery.LearningDeliveryFAMs != null)
                {
                    ILearningDeliveryFAM ldfamWithLatestDateFrom =
                        _learningDeliveryFAMQueryService.GetLearningDeliveryFAMsForType(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT)
                            .Where(fam => fam.LearnDelFAMDateFromNullable.HasValue)
                            .OrderByDescending(f => f.LearnDelFAMDateFromNullable)
                            .FirstOrDefault();

                    if (ldfamWithLatestDateFrom != null &&
                        ldfamWithLatestDateFrom.LearnDelFAMDateFromNullable.HasValue &&
                        ldfamWithLatestDateFrom.LearnDelFAMDateFromNullable.Value > latestACTFAMFrom)
                    {
                        latestACTFAMFrom = ldfamWithLatestDateFrom.LearnDelFAMDateFromNullable.Value;
                        if (learningDelivery.LearnActEndDateNullable.HasValue)
                        {
                            result = learningDelivery;
                        }
                    }
                }
            }

            return result;
        }
    }
}
