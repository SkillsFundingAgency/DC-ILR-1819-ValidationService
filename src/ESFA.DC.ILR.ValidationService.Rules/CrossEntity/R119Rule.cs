using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R119Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _februaryFirst2019 = new DateTime(2019, 02, 01);

        public R119Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R119)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (LearnStartDateConditionMet(learningDelivery.LearnStartDate)
                        && ((learningDelivery.AppFinRecords?.Count ?? 0) > 0))
                {
                    foreach (var appFinRecord in
                        AppFinRecordsConditionMet(
                            learningDelivery.AppFinRecords,
                            learningDelivery.LearnStartDate))
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(
                                learningDelivery.LearnStartDate,
                                appFinRecord.AFinType,
                                appFinRecord.AFinCode,
                                appFinRecord.AFinDate));
                    }
                }
            }
        }

        public IEnumerable<IAppFinRecord> AppFinRecordsConditionMet(
            IReadOnlyCollection<IAppFinRecord> appFinRecords,
            DateTime learnStartDate)
        {
            return appFinRecords
                .Where(
                    f => f != null
                    && f.AFinType.CaseInsensitiveEquals(ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice)
                    && f.AFinDate < learnStartDate);
        }

        public bool LearnStartDateConditionMet(DateTime learnStartDate) => learnStartDate >= _februaryFirst2019;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(
            DateTime learnStartDate,
            string aFinType,
            int? aFinCode,
            DateTime? aFinDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.AFinType, aFinType),
                BuildErrorMessageParameter(PropertyNameConstants.AFinCode, aFinCode),
                BuildErrorMessageParameter(PropertyNameConstants.AFinDate, aFinDate)
            };
        }
    }
}
