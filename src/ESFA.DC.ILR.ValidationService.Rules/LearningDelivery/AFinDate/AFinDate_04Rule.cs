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
    public class AFinDate_04Rule : AbstractRule, IRule<ILearner>
    {
        private const string _aFinType = ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice;

        public AFinDate_04Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AFinDate_04)
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
                if (LearnActEndDateIsKnown(learningDelivery.LearnActEndDateNullable))
                {
                    var tnpRecord = TNPRecordAfterLearnActEndDate(learningDelivery.LearnActEndDateNullable, learningDelivery.AppFinRecords);

                    if (tnpRecord != null)
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(
                                learningDelivery.LearnActEndDateNullable,
                                _aFinType,
                                tnpRecord.AFinDate));
                    }
                }
            }
        }

        public bool LearnActEndDateIsKnown(DateTime? learnActEndDate)
        {
            return learnActEndDate.HasValue;
        }

        public IAppFinRecord TNPRecordAfterLearnActEndDate(DateTime? learnActEndDate, IEnumerable<IAppFinRecord> appFinRecords)
        {
            return
                appFinRecords?
                .FirstOrDefault(f =>
                    f.AFinType.CaseInsensitiveEquals(_aFinType)
                && f.AFinDate > learnActEndDate.Value);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? learnActEndDate, string aFinType, DateTime aFinDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate),
                BuildErrorMessageParameter(PropertyNameConstants.AFinType, aFinType),
                BuildErrorMessageParameter(PropertyNameConstants.AFinDate, aFinDate)
            };
        }
    }
}
