using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AddHours
{
    public class AddHours_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IReadOnlyCollection<int> _fundModels = new HashSet<int>() { FundModelConstants.AdultSkills, FundModelConstants.Apprenticeships, FundModelConstants.OtherAdult };
        private readonly DateTime _learnStartDate = new DateTime(2015, 8, 1);

        public AddHours_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    learningDelivery.AddHoursNullable,
                    learningDelivery.LearnStartDate))
                {
                    HandleValidationError(RuleNameConstants.AddHours_01, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber);
                }
            }
        }

        public bool ConditionMet(int fundModel, int? addHours, DateTime learnStartDate)
        {
            return FundModelConditionMet(fundModel)
                   && AddHoursConditionMet(addHours)
                   && LearnStartDateConditionMet(learnStartDate);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public bool AddHoursConditionMet(int? addHours)
        {
            return addHours.HasValue;
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate)
        {
            return learnStartDate < _learnStartDate;
        }
    }
}
