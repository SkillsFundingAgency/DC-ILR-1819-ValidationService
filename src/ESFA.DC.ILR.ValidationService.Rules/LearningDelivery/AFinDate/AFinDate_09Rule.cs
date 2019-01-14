using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinDate
{
    public class AFinDate_09Rule : AbstractRule, IRule<ILearner>
    {
        private const int _aimType = 1;
        private const int _numberOfYears = -1;

        private readonly IDD07 _dd07;

        public AFinDate_09Rule(IDD07 dd07, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AFinDate_09)
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
                        var aFinRecord = AFinRecordWithDateLessThanLearnStartDate(learningDelivery.LearnStartDate, appFinRecord);

                        if (aFinRecord != null)
                        {
                            HandleValidationError(
                                objectToValidate.LearnRefNumber,
                                learningDelivery.AimSeqNumber,
                                BuildErrorMessageParameters(
                                    aFinRecord.AFinDate,
                                    learningDelivery.LearnStartDate));
                        }
                    }
                }
            }
        }

        public IAppFinRecord AFinRecordWithDateLessThanLearnStartDate(DateTime learnStartDate, IAppFinRecord appFinRecord)
        {
            return appFinRecord.AFinDate < learnStartDate.AddYears(_numberOfYears) ? appFinRecord : null;
        }

        public bool IsAppsStandardOrFramework(int aimType, int? progType)
        {
            return aimType == _aimType
                && _dd07.IsApprenticeship(progType);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime aFinDate, DateTime learnStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AFinDate, aFinDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate)
            };
        }
    }
}
