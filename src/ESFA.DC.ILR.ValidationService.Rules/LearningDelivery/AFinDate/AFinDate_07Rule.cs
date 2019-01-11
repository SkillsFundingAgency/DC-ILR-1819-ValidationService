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
    public class AFinDate_07Rule : AbstractRule, IRule<ILearner>
    {
        private const string _TNP1 = ApprenticeshipFinancialRecord.TotalTrainingPrice;
        private const string _TNP3 = ApprenticeshipFinancialRecord.ResidualTrainingPrice;

        public AFinDate_07Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AFinDate_07)
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
                if (TNP1And3Exists(learningDelivery.AppFinRecords))
                {
                    var tnp1DateEqualToTnp3 = TNP1DateEqualToTNP3Date(learningDelivery.AppFinRecords);

                    if (tnp1DateEqualToTnp3 != null)
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(
                                tnp1DateEqualToTnp3.AFinType,
                                tnp1DateEqualToTnp3.AFinCode,
                                tnp1DateEqualToTnp3.AFinDate));
                    }
                }
            }
        }

        public IAppFinRecord TNP1DateEqualToTNP3Date(IEnumerable<IAppFinRecord> appFinRecords)
        {
            var tnp3Records =
                appFinRecords?
                .Where(af => $"{af.AFinType}{af.AFinCode}".CaseInsensitiveEquals(_TNP3))
                .Select(af => af.AFinDate);

            var tnp1Records = appFinRecords?
               .Where(af => $"{af.AFinType}{af.AFinCode}".CaseInsensitiveEquals(_TNP1));

            return tnp1Records?.Where(af => tnp3Records.Contains(af.AFinDate)).FirstOrDefault();
        }

        public bool TNP1And3Exists(IEnumerable<IAppFinRecord> appFinRecords)
        {
            return
                appFinRecords == null
                ? false
                : appFinRecords.Any(af => $"{af.AFinType}{af.AFinCode}".CaseInsensitiveEquals(_TNP1))
                && appFinRecords.Any(af => $"{af.AFinType}{af.AFinCode}".CaseInsensitiveEquals(_TNP3));
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
