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
    public class AFinDate_08Rule : AbstractRule, IRule<ILearner>
    {
        private const string _TNP2 = ApprenticeshipFinancialRecord.TotalAssessmentPrice;
        private const string _TNP4 = ApprenticeshipFinancialRecord.ResidualAssessmentPrice;

        public AFinDate_08Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AFinDate_08)
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
                if (TNP2And4Exists(learningDelivery.AppFinRecords))
                {
                    var tnp2DateEqualToTnp4 = TNP2DateEqualToTNP4Date(learningDelivery.AppFinRecords);

                    if (tnp2DateEqualToTnp4 != null)
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(
                                tnp2DateEqualToTnp4.AFinType,
                                tnp2DateEqualToTnp4.AFinCode,
                                tnp2DateEqualToTnp4.AFinDate));
                    }
                }
            }
        }

        public IAppFinRecord TNP2DateEqualToTNP4Date(IEnumerable<IAppFinRecord> appFinRecords)
        {
            var tnp4Records =
                appFinRecords?
                .Where(af => $"{af.AFinType}{af.AFinCode}".CaseInsensitiveEquals(_TNP4))
                .Select(af => af.AFinDate);

            var tnp2Records = appFinRecords?
               .Where(af => $"{af.AFinType}{af.AFinCode}".CaseInsensitiveEquals(_TNP2));

            return tnp2Records?.Where(af => tnp4Records.Contains(af.AFinDate)).FirstOrDefault();
        }

        public bool TNP2And4Exists(IEnumerable<IAppFinRecord> appFinRecords)
        {
            return
                appFinRecords == null
                ? false
                : appFinRecords.Any(af => $"{af.AFinType}{af.AFinCode}".CaseInsensitiveEquals(_TNP2))
                && appFinRecords.Any(af => $"{af.AFinType}{af.AFinCode}".CaseInsensitiveEquals(_TNP4));
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
