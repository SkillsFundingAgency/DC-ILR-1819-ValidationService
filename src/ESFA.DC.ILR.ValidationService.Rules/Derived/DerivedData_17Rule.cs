using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_17Rule //: IDerivedData_17Rule
    {
        private readonly IEnumerable<int> _englishOrMathsBasicSkillsTypes = new HashSet<int>(TypeOfLARSBasicSkill.AsEnglishAndMathsBasicSkills);
        private readonly ILARSDataService _larsDataService;

        public DerivedData_17Rule(ILARSDataService larsDataService)
        {
            _larsDataService = larsDataService;
        }

        public bool IsTotalNegotiatedPriceMoreThanCapForStandards(IReadOnlyCollection<ILearningDelivery> learningDeliveries)
        {
            if (learningDeliveries == null)
            {
                return false;
            }

            var filteredLearningDeliveries = learningDeliveries.Where(x => x.StdCodeNullable.HasValue &&
                                                                          x.AimType == TypeOfAim.ProgrammeAim &&
                                                              x.ProgTypeNullable == TypeOfFunding
                                                                  .Age16To19ExcludingApprenticeships &&
                                                              x.FundModel == TypeOfFunding.OtherAdult)
                .ToList();

            var standardAfinTotals = GetAFinTotalValues(filteredLearningDeliveries);

            foreach (var standardCode in standardAfinTotals.Keys)
            {
                var earliestStartDate = filteredLearningDeliveries.Where(x => x.StdCodeNullable == standardCode)
                    .OrderBy(x => x.LearnStartDate).FirstOrDefault()?.LearnStartDate;

                if (IsCapMoreThanTotalStandardsValue(standardCode, standardAfinTotals[standardCode], earliestStartDate))
                {
                    return true;
                }

                var earliestOrigStartDate = filteredLearningDeliveries.Where(x => x.StdCodeNullable == standardCode && x.OrigLearnStartDateNullable.HasValue)
                    .OrderBy(x => x.OrigLearnStartDateNullable).FirstOrDefault()?.OrigLearnStartDateNullable;

                if (IsCapMoreThanTotalStandardsValue(standardCode, standardAfinTotals[standardCode], earliestOrigStartDate))
                {
                    return true;
                }
            }

            return false;
        }

        public Dictionary<int, int> GetAFinTotalValues(List<ILearningDelivery> list)
        {
            var standardAfinTotals = new Dictionary<int, int>();

            foreach (var learningDelivery in list)
            {
                if (learningDelivery.AppFinRecords != null)
                {
                    var aFinCode1Value = GetLatestAppFinRecord(learningDelivery.AppFinRecords, 1);
                    var aFinCode2Value = GetLatestAppFinRecord(learningDelivery.AppFinRecords, 2);

                    standardAfinTotals[learningDelivery.StdCodeNullable.Value] = +(aFinCode1Value + aFinCode2Value);
                }
            }

            return standardAfinTotals;
        }

        public int GetLatestAppFinRecord(IReadOnlyCollection<IAppFinRecord> appFinRecords, int appFinCode)
        {
            return appFinRecords.Where(x =>
                    x.AFinType.CaseInsensitiveEquals(ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice) &&
                    x.AFinCode == appFinCode)
                .OrderByDescending(x => x.AFinDate)
                .Select(x => x.AFinAmount)
                .FirstOrDefault();
        }

        public bool IsCapMoreThanTotalStandardsValue(int standardCode, int totalStandardsValue, DateTime? startDate)
        {
            if (startDate != null)
            {
                var fundingCap =
                    _larsDataService.GetCoreGovContributionCapForStandard(
                        standardCode,
                        startDate.Value);

                if (((2 / 3) * totalStandardsValue) > fundingCap)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
