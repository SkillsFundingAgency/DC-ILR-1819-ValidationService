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
    public class AFinDate_05Rule : AbstractRule, IRule<ILearner>
    {
        private const string _TNP1 = ApprenticeshipFinanicalRecord.TotalTrainingPrice;
        private const string _TNP3 = ApprenticeshipFinanicalRecord.ResidualTrainingPrice;

        public AFinDate_05Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AFinDate_05)
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
                if (TNP3Exists(learningDelivery.AppFinRecords))
                {
                    var tnp1RecordLaterThanTnp3 = TNP1RecordLaterThanTNP3Record(learningDelivery.AppFinRecords);

                    if (tnp1RecordLaterThanTnp3 != null)
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(
                                tnp1RecordLaterThanTnp3.AFinType,
                                tnp1RecordLaterThanTnp3.AFinCode,
                                tnp1RecordLaterThanTnp3.AFinDate));
                    }
                }
            }
        }

        public IAppFinRecord TNP1RecordLaterThanTNP3Record(IEnumerable<IAppFinRecord> appFinRecords)
        {
            var tnp3Records =
                appFinRecords?
                .Where(af => $"{af.AFinType}{af.AFinCode}".CaseInsensitiveEquals(_TNP3))
                .Select(af => af.AFinDate);

            var tnp1Records = appFinRecords?
               .Where(af => $"{af.AFinType}{af.AFinCode}".CaseInsensitiveEquals(_TNP1));

            return tnp1Records?.Where(af => af.AFinDate > tnp3Records?.Max()).FirstOrDefault();
        }

        public bool TNP3Exists(IEnumerable<IAppFinRecord> appFinRecords)
        {
            return
                appFinRecords == null
                ? false
                : appFinRecords.Any(af => $"{af.AFinType}{af.AFinCode}".CaseInsensitiveEquals(_TNP3));
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
