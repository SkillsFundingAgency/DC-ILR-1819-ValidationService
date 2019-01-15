using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_34Rule : AbstractRule, IRule<ILearner>
    {
        private const double DaysInYear = 365.242199;
        private const int MinAge = 19;
        private const int MaxAge = 24;

        private readonly IAcademicYearDataService _academicYearDataService;

        public DateOfBirth_34Rule(
            IAcademicYearDataService academicYearDataService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DateOfBirth_34)
        {
            _academicYearDataService = academicYearDataService;
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="learner">The object to validate.</param>
        public void Validate(ILearner learner)
        {
            if (learner?.DateOfBirthNullable == null || learner.LearnerFAMs == null || learner.LearningDeliveries == null)
            {
                return;
            }

            var age = Convert.ToInt32(Math.Floor((_academicYearDataService.AugustThirtyFirst() - (learner.DateOfBirthNullable ?? DateTime.MinValue)).TotalDays / DaysInYear));
            if (age < MinAge || age > MaxAge)
            {
                return;
            }

            if (learner.LearnerFAMs.All(lf => !lf.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.HNS)) ||
                learner.LearnerFAMs.Any(lf => lf.LearnFAMType.CaseInsensitiveEquals(LearnerFAMTypeConstants.EHC)))
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (learningDelivery.FundModel == TypeOfFunding.Age16To19ExcludingApprenticeships)
                {
                    RaiseValidationMessage(learner, learningDelivery);
                }
            }
        }

        private void RaiseValidationMessage(ILearner learner, ILearningDelivery learningDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnFAMCode, LearnerFAMTypeConstants.HNS),
                BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, learner.DateOfBirthNullable),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, TypeOfFunding.Age16To19ExcludingApprenticeships)
            };

            HandleValidationError(learner.LearnRefNumber, learningDelivery.AimSeqNumber, parameters);
        }
    }
}
