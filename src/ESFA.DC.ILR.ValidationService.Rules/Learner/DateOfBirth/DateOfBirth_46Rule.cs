﻿using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_46Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<long?> _fundModels = new HashSet<long?>() { 36, 81 };
        private readonly DateTime _firstAug2016 = new DateTime(2016, 08, 01);

        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public DateOfBirth_46Rule(IDateTimeQueryService dateTimeQueryService, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
           : base(validationErrorHandler, RuleNameConstants.DateOfBirth_46)
        {
            _dateTimeQueryService = dateTimeQueryService;

            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries != null)
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (ConditionMet(
                        learningDelivery.ProgTypeNullable,
                        learningDelivery.FundModel,
                        learningDelivery.LearnStartDate,
                        learningDelivery.LearnPlanEndDate,
                        learningDelivery.AimType,
                        objectToValidate.DateOfBirthNullable,
                        learningDelivery.LearningDeliveryFAMs))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(objectToValidate.DateOfBirthNullable, learningDelivery.ProgTypeNullable));
                        return;
                    }
                }
            }
        }

        public bool ConditionMet(int? progType, int fundModel, DateTime learnStartDate, DateTime learnPlanEndDate, int aimType, DateTime? dateOfBirth, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return FundModelConditionMet(fundModel)
            && LearnStartDateConditionMet(learnStartDate)
            && AimTypeConditionMet(aimType)
            && ProgTypeConditionMet(progType)
            && DateOfBirthConditionMet(dateOfBirth, learnStartDate)
            && LearnPlanEndDateConditionMet(learnStartDate, learnPlanEndDate)
            && LearningDeliveryFAMConditionMet(learningDeliveryFAMs);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate >= _firstAug2016;
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == 1;
        }

        public bool ProgTypeConditionMet(int? progType)
        {
            return progType.HasValue
                && progType == 25;
        }

        public bool DateOfBirthConditionMet(DateTime? dateOfBirth, DateTime learnStartDate)
        {
            return dateOfBirth.HasValue
                && _dateTimeQueryService.YearsBetween((DateTime)dateOfBirth, learnStartDate) >= 16;
        }

        public bool LearnPlanEndDateConditionMet(DateTime learnStartDate, DateTime learnPlanEndDate)
        {
            return _dateTimeQueryService.DaysBetween(learnStartDate, learnPlanEndDate) < 372;
        }

        public bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES");
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? dateOfBirth, int? progType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType)
            };
        }
    }
}
