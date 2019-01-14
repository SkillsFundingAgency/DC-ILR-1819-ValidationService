using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType
{
    public class AFinType_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryAppFinRecordQueryService _learningDeliveryAppFinRecordQueryService;

        public AFinType_01Rule(
            ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AFinType_01)
        {
            _learningDeliveryAppFinRecordQueryService = learningDeliveryAppFinRecordQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.AimType,
                    learningDelivery.AppFinRecords))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber);
                }
            }
        }

        public bool ConditionMet(int fundModel, int? progType, int aimType, IEnumerable<IAppFinRecord> appFinRecords)
        {
            return FundModelConditionMet(fundModel)
                   && ProgTypeConditionMet(progType)
                   && AimTypeConditionMet(aimType)
                   && AppFinConditionMet(appFinRecords);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == 81;
        }

        public bool ProgTypeConditionMet(int? progType)
        {
            return progType == 25;
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == 1;
        }

        public bool AppFinConditionMet(IEnumerable<IAppFinRecord> appFinRecords)
        {
            return !_learningDeliveryAppFinRecordQueryService.HasAnyLearningDeliveryAFinCodeForType(appFinRecords, ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice, 1);
        }
    }
}
