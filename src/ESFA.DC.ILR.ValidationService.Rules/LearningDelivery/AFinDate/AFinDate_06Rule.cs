using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinDate
{
    public class AFinDate_06Rule : AbstractRule, IRule<ILearner>
    {
        private const string _TNP2 = ApprenticeshipFinancialRecord.TotalAssessmentPrice;
        private const string _TNP4 = ApprenticeshipFinancialRecord.ResidualAssessmentPrice;

        public AFinDate_06Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AFinDate_06)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (TNP4Exists(learningDelivery.AppFinRecords))
                {
                    var tnp2RecordLaterThanTnp4 = TNP2RecordLaterThanTNP4Record(learningDelivery.AppFinRecords);

                    if (tnp2RecordLaterThanTnp4 != null)
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(
                                tnp2RecordLaterThanTnp4.AFinType,
                                tnp2RecordLaterThanTnp4.AFinCode,
                                tnp2RecordLaterThanTnp4.AFinDate));
                    }
                }
            }
        }

        public IAppFinRecord TNP2RecordLaterThanTNP4Record(IEnumerable<IAppFinRecord> appFinRecords)
        {
            var tnp4Records =
                appFinRecords?
                .Where(af => $"{af.AFinType}{af.AFinCode}".CaseInsensitiveEquals(_TNP4))
                .Select(af => af.AFinDate);

            var tnp2Records = appFinRecords?
               .Where(af => $"{af.AFinType}{af.AFinCode}".CaseInsensitiveEquals(_TNP2));

            return tnp2Records?.Where(af => af.AFinDate > tnp4Records?.Max()).FirstOrDefault();
        }

        public bool TNP4Exists(IEnumerable<IAppFinRecord> appFinRecords)
        {
            return
                appFinRecords == null
                ? false
                : appFinRecords.Any(af => $"{af.AFinType}{af.AFinCode}".CaseInsensitiveEquals(_TNP4));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string aFinType, int aFinCode, DateTime aFinDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AFinType, aFinType),
                BuildErrorMessageParameter(PropertyNameConstants.AFinCode, aFinCode),
                BuildErrorMessageParameter(PropertyNameConstants.AFinDate, aFinDate)
            };
        }
    }
}
