using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType
{
    public class AFinType_10Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<int> _aFinCodes = new HashSet<int> { 2, 4 };

        private readonly ILearningDeliveryAppFinRecordQueryService _learningDeliveryAppFinRecordQueryService;

        public AFinType_10Rule(ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService, IValidationErrorHandler validationErrorHandler)
           : base(validationErrorHandler, RuleNameConstants.AFinType_10)
        {
            _learningDeliveryAppFinRecordQueryService = learningDeliveryAppFinRecordQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (learningDelivery.AppFinRecords != null)
                {
                    if (ConditionMet(
                        learningDelivery.FundModel,
                        learningDelivery.ProgTypeNullable,
                        learningDelivery.AimType,
                        learningDelivery.AppFinRecords))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber);
                    }
                }
            }
        }

        public bool ConditionMet(int fundModel, int? progType, int aimType, IEnumerable<IAppFinRecord> appFinRecords)
        {
            return FundModelConditionMet(fundModel)
                && ApprenticeshipStandardConditionMet(progType, aimType)
                && AppFinRecordConditionMet(appFinRecords);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel != 99;
        }

        public bool ApprenticeshipStandardConditionMet(int? progType, int aimType)
        {
            if (progType != null)
            {
                return progType == 25 && aimType == 1;
            }

            return false;
        }

        public bool AppFinRecordConditionMet(IEnumerable<IAppFinRecord> appFinRecords)
        {
            return !_learningDeliveryAppFinRecordQueryService.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", _aFinCodes);
        }
    }
}
