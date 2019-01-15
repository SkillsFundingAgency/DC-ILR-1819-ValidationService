using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_40Rule : AbstractRule, IRule<ILearner>
    {
        private const int MinAge = 19;
        private const int MinimumContractMonths = 12;

        private const int ProgrammeType = TypeOfLearningProgramme.ApprenticeshipStandard;
        private const int AimType = TypeOfAim.ProgrammeAim;

        private readonly DateTime _ruleEndDate = new DateTime(2016, 7, 31);
        private readonly int[] _fundModels = { TypeOfFunding.AdultSkills, TypeOfFunding.OtherAdult };

        private readonly IDateTimeQueryService _dateTimeQueryService;

        public DateOfBirth_40Rule(
            IDateTimeQueryService dateTimeQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_40)
        {
            _dateTimeQueryService = dateTimeQueryService;
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="learner">The object to validate.</param>
        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (learningDelivery.LearnStartDate > _ruleEndDate)
                {
                    continue;
                }

                var age = _dateTimeQueryService.AgeAtGivenDate(
                    learner.DateOfBirthNullable ?? DateTime.MinValue,
                    learningDelivery.LearnStartDate);

                if (age < MinAge)
                {
                    continue;
                }

                if (_fundModels.All(fm => fm != learningDelivery.FundModel) ||
                    (learningDelivery.ProgTypeNullable ?? -1) != ProgrammeType ||
                    learningDelivery.AimType != AimType)
                {
                    continue;
                }

                if (learningDelivery.LearningDeliveryFAMs
                        ?.Any(ldf => ldf.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.RES)) ?? false)
                {
                    continue;
                }

                if (learningDelivery.LearnStartDate.AddMonths(MinimumContractMonths) > (learningDelivery.LearnActEndDateNullable ?? DateTime.MaxValue))
                {
                    RaiseValidationMessage(learner, learningDelivery);
                }
            }
        }

        private void RaiseValidationMessage(ILearner learner, ILearningDelivery learningDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learningDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learningDelivery.LearnActEndDateNullable)
            };

            HandleValidationError(learner.LearnRefNumber, learningDelivery.AimSeqNumber, parameters);
        }
    }
}