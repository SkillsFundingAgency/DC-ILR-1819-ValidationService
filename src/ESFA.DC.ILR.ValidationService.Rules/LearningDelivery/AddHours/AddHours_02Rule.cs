﻿using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AddHours
{
    public class AddHours_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IReadOnlyCollection<int> _fundModels = new HashSet<int>() { FundModelConstants.CommunityLearning, FundModelConstants.SixteenToNineteen, FundModelConstants.OtherSixteenToNineteen, FundModelConstants.NonFunded };

        public AddHours_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    learningDelivery.AddHoursNullable))
                {
                    HandleValidationError(RuleNameConstants.AddHours_02, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber);
                }
            }
        }

        public bool ConditionMet(int fundModel, int? addHours)
        {
            return addHours.HasValue
                   && _fundModels.Contains(fundModel);
        }
    }
}