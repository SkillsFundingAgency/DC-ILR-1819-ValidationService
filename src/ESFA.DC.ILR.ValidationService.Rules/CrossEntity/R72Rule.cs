﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R72Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryAppFinRecordQueryService _learningDeliveryAppFinRecordQueryService;
        private readonly IDerivedData_17Rule _dd17;
        private readonly HashSet<int> AFinCodes1And2 = new HashSet<int>() { 1, 2 };
        private readonly int AFinCode3 = 3;

        public R72Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService,
            IDerivedData_17Rule dd17)
            : base(validationErrorHandler, RuleNameConstants.R72)
        {
            _learningDeliveryAppFinRecordQueryService = learningDeliveryAppFinRecordQueryService;
            _dd17 = dd17;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            var filteredLearningDeliveries = objectToValidate.LearningDeliveries.Where(
                    x => x.StdCodeNullable.HasValue &&
                    x.AimType == TypeOfAim.ProgrammeAim &&
                    x.ProgTypeNullable == TypeOfFunding.Age16To19ExcludingApprenticeships &&
                    x.FundModel == TypeOfFunding.OtherAdult).ToList();

            var standardPMRTotalValues = filteredLearningDeliveries.GroupBy(
                x => new
                {
                    x.StdCodeNullable.Value,
                    TotalAFin1And2 = x.AppFinRecords?.Where(
                        fin => fin.AFinType.CaseInsensitiveEquals(ApprenticeshipFinancialRecord.Types.PaymentRecord) &&
                               AFinCodes1And2.Contains(fin.AFinCode)).Sum(s => s.AFinAmount),
                    TotalAFin3 = x.AppFinRecords?.Where(
                        fin => fin.AFinType.CaseInsensitiveEquals(ApprenticeshipFinancialRecord.Types.PaymentRecord) &&
                               fin.AFinCode == AFinCode3).Sum(s => s.AFinAmount)
                }).Select(x => new
                {
                    StandardCode = x.Key.Value,
                    TotalPMRValue = x.Key.TotalAFin1And2 - x.Key.TotalAFin3
                });

            foreach (var standardPMR in standardPMRTotalValues)
            {
                if (ConditionMet(standardPMR.StandardCode, standardPMR.TotalPMRValue, filteredLearningDeliveries))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        null,
                        BuildErrorMessageParameters(objectToValidate.LearnRefNumber, standardPMR.StandardCode));
                }
            }
        }

        public bool ConditionMet(int standardCode, int? totalPMRValue, IReadOnlyCollection<ILearningDelivery> learningDeliveries)
        {
            if (!totalPMRValue.HasValue)
            {
                return false;
            }

            var standardLearningDeliveries = learningDeliveries.Where(x => x.StdCodeNullable == standardCode).ToList();

            if (!_dd17.IsTotalNegotiatedPriceMoreThanCapForStandard(standardLearningDeliveries, standardCode))
            {
                var standardTNPTotal = _learningDeliveryAppFinRecordQueryService.GetTotalTNPPriceForLatestAppFinRecordsForLearning(standardLearningDeliveries);

                return totalPMRValue > ((1m / 3m) * standardTNPTotal);
            }

            return false;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnRefNumber, int standardCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnRefNumber, learnRefNumber),
                BuildErrorMessageParameter(PropertyNameConstants.AimType, TypeOfAim.ProgrammeAim),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, TypeOfFunding.Age16To19ExcludingApprenticeships),
                BuildErrorMessageParameter(PropertyNameConstants.StdCode, standardCode),
            };
        }
    }
}
