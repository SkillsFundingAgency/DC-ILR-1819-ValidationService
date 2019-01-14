using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinDate
{
    public class AFinDate_12Rule : AbstractRule, IRule<ILearner>
    {
        private const int _aimType = 1;
        private const int _numberOfYears = 1;

        private readonly IDD07 _dd07;

        public AFinDate_12Rule(IDD07 dd07, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AFinDate_12)
        {
            _dd07 = dd07;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (learningDelivery.AppFinRecords != null
                    && IsAppsStandardOrFramework(learningDelivery.AimType, learningDelivery.ProgTypeNullable))
                {
                    foreach (var appFinRecord in learningDelivery.AppFinRecords)
                    {
                        var aFinRecord = AFinRecordWithDateGreaterThanLearnActEndDate(learningDelivery.LearnActEndDateNullable, appFinRecord);

                        if (aFinRecord != null)
                        {
                            HandleValidationError(
                                objectToValidate.LearnRefNumber,
                                learningDelivery.AimSeqNumber,
                                BuildErrorMessageParameters(
                                    aFinRecord.AFinDate,
                                    learningDelivery.LearnActEndDateNullable));
                        }
                    }
                }
            }
        }

        public IAppFinRecord AFinRecordWithDateGreaterThanLearnActEndDate(DateTime? learnActEndDate, IAppFinRecord appFinRecord)
        {
            if (learnActEndDate != null)
            {
                return appFinRecord.AFinDate > learnActEndDate.Value.AddYears(_numberOfYears) ? appFinRecord : null;
            }

            return null;
        }

        public bool IsAppsStandardOrFramework(int aimType, int? progType)
        {
            return aimType == _aimType
                && _dd07.IsApprenticeship(progType);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime aFinDate, DateTime? learnActEndDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AFinDate, aFinDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate)
            };
        }
    }
}
