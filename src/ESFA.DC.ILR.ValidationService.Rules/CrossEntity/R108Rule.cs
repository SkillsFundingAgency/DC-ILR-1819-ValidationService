using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R108Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IEnumerable<int> _fundModels = new HashSet<int>() { FundModelConstants.CommunityLearning, FundModelConstants.AdultSkills, FundModelConstants.ESF, FundModelConstants.OtherAdult };

        private readonly IFileDataService _fileDataService;

        public R108Rule(
            IValidationErrorHandler validationErrorHandler,
            IFileDataService fileDataService)
            : base(validationErrorHandler, RuleNameConstants.R108)
        {
            _fileDataService = fileDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            DateTime? learnActEndDateLatest = null;
            DateTime? outStartDate = null;
            string ldapLearnRefNumber = string.Empty;

            if (objectToValidate == null
                || !FundModelConditionMet(objectToValidate.LearningDeliveries)
                || !AllAimsClosedConditionMet(objectToValidate.LearningDeliveries)
                || !CompStatusConditionMet(objectToValidate.LearningDeliveries, out learnActEndDateLatest)
                || (!learnActEndDateLatest.HasValue || !FilePreparationDateConditionMet(DateTime.Parse(learnActEndDateLatest?.ToString())))
                || (!learnActEndDateLatest.HasValue || !DPOutComeConditionMet(
                    objectToValidate.LearnRefNumber,
                    DateTime.Parse(learnActEndDateLatest?.ToString()),
                    out ldapLearnRefNumber,
                    out outStartDate)))
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.ProgTypeNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.FundModel,
                        learningDelivery.CompStatus,
                        learningDelivery.LearnActEndDateNullable,
                        ldapLearnRefNumber,
                        outStartDate));
                }
            }
        }

        public bool ConditionMet(int? progTypeNullable)
        {
            return ProgTypeConditionMet(progTypeNullable);
        }

        public bool ProgTypeConditionMet(int? progTypeNullable)
        {
            return !((progTypeNullable ?? -1) == 24 || (progTypeNullable ?? -1) == 25);
        }

        public bool FundModelConditionMet(IReadOnlyCollection<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries.Where(ld => _fundModels.Contains(ld.FundModel)).Count() > 0;
        }

        public bool AllAimsClosedConditionMet(IReadOnlyCollection<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries.Where(ld => ld.LearnActEndDateNullable != null).Count() == learningDeliveries.Count();
        }

        public bool CompStatusConditionMet(IReadOnlyCollection<ILearningDelivery> learningDeliveries, out DateTime? learnActEndDateLatest)
        {
            bool compStatusCondition = false;
            learnActEndDateLatest = null;

            ILearningDelivery learningDelivery = learningDeliveries.Where(ld => ld.LearnActEndDateNullable.HasValue).OrderByDescending(ld => ld.LearnActEndDateNullable).FirstOrDefault();
            if (learningDelivery != null)
            {
                learnActEndDateLatest = learningDelivery.LearnActEndDateNullable;
                compStatusCondition = learningDelivery.CompStatus != 6;
            }

            return compStatusCondition;
        }

        public bool FilePreparationDateConditionMet(DateTime learnActEndDateLatest)
        {
            return _fileDataService.FilePreparationDate() >= learnActEndDateLatest.AddMonths(2);
        }

        public bool DPOutComeConditionMet(string learnRefNumber, DateTime learnActEndDateLatest, out string ldapLearnRefNumber, out DateTime? outStartDate)
        {
            ldapLearnRefNumber = string.Empty;
            outStartDate = null;
            bool conditionMet = false;

            var lDAP = _fileDataService.LearnerDestinationAndProgressionsForLearnRefNumber(learnRefNumber);

            if (lDAP != null)
            {
                ldapLearnRefNumber = lDAP.LearnRefNumber;
                outStartDate = lDAP.DPOutcomes?.OrderByDescending(dp => dp.OutStartDate).Select(dp => dp.OutStartDate).FirstOrDefault();
                IDPOutcome dpOutCome = lDAP.DPOutcomes?.Where(dpo => dpo.OutStartDate >= learnActEndDateLatest).FirstOrDefault();
                if (dpOutCome == null)
                {
                    conditionMet = true;
                }
            }

            return conditionMet;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnRefNumber, int fundModel, int compStatus, DateTime? learnActEndDateNullable, string ldapLearnRefNumber, DateTime? outStartDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnRefNumber, learnRefNumber),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.CompStatus, compStatus),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDateNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearningDestinationAndProgressionLearnRefNumber, ldapLearnRefNumber),
                BuildErrorMessageParameter(PropertyNameConstants.OutStartDate, outStartDate)
            };
        }
    }
}
