using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinDate
{
    public class AFinDate_02Rule : AbstractRule, IRule<ILearner>
    {
        private const int _yearsAfter = 1;
        private const string _dd19ErrorParameter = "DD19";
        private readonly IDerivedData_19Rule _dd19;

        public AFinDate_02Rule(IDerivedData_19Rule dd19, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AFinDate_02)
        {
            _dd19 = dd19;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (learningDelivery.AppFinRecords == null)
                {
                    continue;
                }

                var latestLearnedPlannedEndDate = _dd19.Derive(objectToValidate.LearningDeliveries, learningDelivery);

                var aFinDatesOneYearAfter = latestLearnedPlannedEndDate == null
                    ? new List<DateTime>()
                    : AFInDatesOneYearAfterProgramme(learningDelivery.AppFinRecords, latestLearnedPlannedEndDate.Value);

                if (aFinDatesOneYearAfter.Any())
                {
                    foreach (var aFinDate in aFinDatesOneYearAfter)
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(aFinDate, _dd19ErrorParameter));
                    }
                }
            }
        }

        public IReadOnlyCollection<DateTime> AFInDatesOneYearAfterProgramme(IEnumerable<IAppFinRecord> appFinRecords, DateTime learnPlanEndDate)
        {
            var aFinDates = new List<DateTime>();

            foreach (var appFinRecord in appFinRecords)
            {
                if (appFinRecord.AFinDate > learnPlanEndDate.AddYears(_yearsAfter))
                {
                    aFinDates.Add(appFinRecord.AFinDate);
                }
            }

            return aFinDates;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime aFinDate, string dd19)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AFinDate, aFinDate),
                BuildErrorMessageParameter(PropertyNameConstants.DD19, dd19)
            };
        }
    }
}
