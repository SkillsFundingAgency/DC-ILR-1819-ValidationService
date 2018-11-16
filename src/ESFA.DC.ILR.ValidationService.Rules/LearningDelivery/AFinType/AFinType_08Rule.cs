using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType
{
    public class AFinType_08Rule : AbstractRule, IRule<ILearner>
    {
        private readonly string _aFinType = ApprenticeshipFinanicalRecord.Types.TotalNegotiatedPrice;
        private readonly HashSet<int> _appFinCodes = new HashSet<int> { 3, 4 };

        private readonly ILearningDeliveryAppFinRecordQueryService _appFindRecordQueryService;

        public AFinType_08Rule(ILearningDeliveryAppFinRecordQueryService appFindRecordQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AFinType_08)
        {
            _appFindRecordQueryService = appFindRecordQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FundModel, learningDelivery.AppFinRecords))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(learningDelivery.FundModel));
                }
            }
        }

        public bool ConditionMet(int fundModel, IEnumerable<IAppFinRecord> appFinRecords)
        {
            return FundModelConditionMet(fundModel)
                && AppFinRecordConditionMet(appFinRecords);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel != 36;
        }

        public bool AppFinRecordConditionMet(IEnumerable<IAppFinRecord> appFinRecords)
        {
            return _appFindRecordQueryService.HasAnyLearningDeliveryAFinCodesForType(
                appFinRecords, _aFinType, _appFinCodes);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}
