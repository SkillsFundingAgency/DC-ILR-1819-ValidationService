using System;
using System.Collections.Generic;
using System.Globalization;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate
{
    public class OrigLearnStartDate_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly List<int> FundModels = new List<int> { 35, 36, 81, 99 };

        public OrigLearnStartDate_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.OrigLearnStartDate_01)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.LearnStartDate,
                    learningDelivery.OrigLearnStartDateNullable,
                    learningDelivery.FundModel))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.LearnStartDate, learningDelivery.OrigLearnStartDateNullable));
                }
            }
        }

        public bool ConditionMet(DateTime learnStartDate, DateTime? originalLearnStartDate, int fundModel)
        {
            return FundModelConditionMet(fundModel) &&
                   OriginalLearnStartDateConditionMet(learnStartDate, originalLearnStartDate);
        }

        public bool OriginalLearnStartDateConditionMet(DateTime learnStartDate, DateTime? originalLearnStartDate)
        {
            return originalLearnStartDate.HasValue &&
                   originalLearnStartDate < learnStartDate.AddYears(-10);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return FundModels.Contains(fundModel);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime learnStartDate, DateTime? originalLearnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate.ToString("d", new CultureInfo("en-GB"))),
                BuildErrorMessageParameter(PropertyNameConstants.OrigLearnStartDate, originalLearnStartDate?.ToString("d", new CultureInfo("en-GB")))
            };
        }
    }
}
