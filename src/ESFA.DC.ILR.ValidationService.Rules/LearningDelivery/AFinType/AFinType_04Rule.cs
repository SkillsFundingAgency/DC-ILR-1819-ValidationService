using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType
{
    public class AFinType_04Rule : AbstractRule, IRule<ILearner>
    {
        public AFinType_04Rule(
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AFinType_04)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.AppFinRecords,
                    learningDelivery.FundModel,
                    learningDelivery.AimType,
                    learningDelivery.ProgTypeNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.AimType, learningDelivery.ProgTypeNullable));
                }
            }
        }

        public bool ConditionMet(IEnumerable<IAppFinRecord> appFinRecords, int fundModel, int aimType, int? progType)
        {
            return AppFinRecordConditionMet(appFinRecords)
                && AimConditionMet(fundModel, aimType, progType);
        }

        public bool AppFinRecordConditionMet(IEnumerable<IAppFinRecord> appFinRecords)
        {
            return appFinRecords != null;
        }

        public bool AimConditionMet(int fundModel, int aimType, int? progType)
        {
            return !(fundModel == 81 && progType == 25 && aimType == 1)
                   || !(fundModel == 36 && aimType == 1);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, int aimType, int? progType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
            };
        }
    }
}
