using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType
{
    public class AFinType_14Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryAppFinRecordQueryService _learningDeliveryAppFinRecordQueryService;

        public AFinType_14Rule(ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AFinType_14)
        {
            _learningDeliveryAppFinRecordQueryService = learningDeliveryAppFinRecordQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                var aFinCode = learningDelivery.AppFinRecords?.
                    Where(a => a.AFinType == ApprenticeshipFinanicalRecord.Types.TotalNegotiatedPrice
                    && (a.AFinCode == 1 || a.AFinCode == 3))
                    .Select(a => a.AFinCode)
                    .FirstOrDefault();

                if (ConditionMet(
                    learningDelivery.AimType,
                    learningDelivery.AppFinRecords,
                    aFinCode))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(ApprenticeshipFinanicalRecord.Types.TotalNegotiatedPrice, aFinCode));
                }
            }
        }

        public bool ConditionMet(int aimType, IEnumerable<IAppFinRecord> appFinRecords, int? aFinCode)
        {
            return TNPConditionMet(aFinCode)
                && PMRConditionMet(aimType, appFinRecords);
        }

        public bool TNPConditionMet(int? aFinCode)
        {
            return aFinCode == 1 || aFinCode == 3;
        }

        public bool PMRConditionMet(int aimType, IEnumerable<IAppFinRecord> appFinRecords)
        {
            return
            !(_learningDeliveryAppFinRecordQueryService
            .HasAnyLearningDeliveryAFinCodeForType(appFinRecords, ApprenticeshipFinanicalRecord.Types.PaymentRecord, 1)
              && aimType == 1);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string aFinType, int? aFinCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AFinType, aFinType),
                BuildErrorMessageParameter(PropertyNameConstants.AFinCode, aFinCode)
            };
        }
    }
}
