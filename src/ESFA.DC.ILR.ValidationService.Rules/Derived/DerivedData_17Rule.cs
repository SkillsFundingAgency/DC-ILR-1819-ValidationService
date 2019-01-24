using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_17Rule : IDerivedData_17Rule
    {
        private readonly ILARSDataService _larsDataService;
        private readonly ILearningDeliveryAppFinRecordQueryService _learningDeliveryAppFinRecordQueryService;

        public DerivedData_17Rule(ILARSDataService larsDataService, ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService)
        {
            _larsDataService = larsDataService;
            _learningDeliveryAppFinRecordQueryService = learningDeliveryAppFinRecordQueryService;
        }

        public bool IsTotalNegotiatedPriceMoreThanCapForStandardLearningDeliveries(IReadOnlyCollection<ILearningDelivery> standardLearningDeliveries)
        {
            return IsTotalNegotiatedPriceMoreThanCapForStandard(standardLearningDeliveries, null);
        }

        public bool IsTotalNegotiatedPriceMoreThanCapForStandard(IReadOnlyCollection<ILearningDelivery> learningDeliveries, int? standardCode)
        {
            if (learningDeliveries == null)
            {
                return false;
            }

            var filteredLearningDeliveries = learningDeliveries.Where(
                    x => x.StdCodeNullable.HasValue &&
                    (!standardCode.HasValue || (x.StdCodeNullable.Value == standardCode)) &&
                    x.AimType == TypeOfAim.ProgrammeAim &&
                    x.ProgTypeNullable == TypeOfFunding.Age16To19ExcludingApprenticeships &&
                    x.FundModel == TypeOfFunding.OtherAdult)
                    .ToList();

            if (filteredLearningDeliveries.Any())
            {
                var standardAfinTotals = GetAFinTotalValues(filteredLearningDeliveries);

                foreach (var standardCodeKey in standardAfinTotals.Keys)
                {
                    var applicableDate = GetApplicableDateForCapChecking(filteredLearningDeliveries, standardCodeKey);

                    if (IsAFilTotalMoreThanCapValue(standardCodeKey, standardAfinTotals[standardCodeKey], applicableDate))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public DateTime? GetApplicableDateForCapChecking(List<ILearningDelivery> learningDeliveries, int standardCode)
        {
            var earliestStartDate = learningDeliveries.Where(x => x.StdCodeNullable == standardCode)
                .OrderBy(x => x.LearnStartDate).FirstOrDefault()?.LearnStartDate;

            var earliestOrigStartDate = learningDeliveries
                .Where(x => x.StdCodeNullable == standardCode && x.OrigLearnStartDateNullable.HasValue)
                .OrderBy(x => x.OrigLearnStartDateNullable).FirstOrDefault()?.OrigLearnStartDateNullable;

            var applicableDate = earliestOrigStartDate.HasValue && earliestStartDate > earliestOrigStartDate.Value
                ? earliestOrigStartDate.Value
                : earliestStartDate;

            return applicableDate;
        }

        public Dictionary<int, int> GetAFinTotalValues(List<ILearningDelivery> learningDeliveries)
        {
            var standardAfinTotals = new Dictionary<int, int>();

            if (learningDeliveries != null)
            {
                foreach (var learningDelivery in learningDeliveries)
                {
                    if (learningDelivery.AppFinRecords != null)
                    {
                        var aFinCode1Value = _learningDeliveryAppFinRecordQueryService.GetLatestAppFinRecord(
                            learningDelivery.AppFinRecords,
                            ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                            1)?.AFinAmount;

                        var aFinCode2Value = _learningDeliveryAppFinRecordQueryService.GetLatestAppFinRecord(
                            learningDelivery.AppFinRecords,
                            ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                            2)?.AFinAmount;

                        var total = aFinCode1Value.GetValueOrDefault() + aFinCode2Value.GetValueOrDefault();

                        if (standardAfinTotals.ContainsKey(learningDelivery.StdCodeNullable.Value))
                        {
                            standardAfinTotals[learningDelivery.StdCodeNullable.Value] += total;
                        }
                        else
                        {
                            standardAfinTotals[learningDelivery.StdCodeNullable.Value] = total;
                        }
                    }
                }
            }

            return standardAfinTotals;
        }

        public bool IsAFilTotalMoreThanCapValue(int standardCode, int totalStandardsValue, DateTime? startDate)
        {
            if (startDate != null)
            {
                var fundingCap = _larsDataService.GetCoreGovContributionCapForStandard(standardCode, startDate.Value);

                if (!fundingCap.HasValue)
                {
                    return false;
                }

                if (((2m / 3m) * totalStandardsValue) > fundingCap)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
