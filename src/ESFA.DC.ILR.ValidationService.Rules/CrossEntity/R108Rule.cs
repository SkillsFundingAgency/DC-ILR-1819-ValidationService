using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R108Rule : AbstractRule, IRule<IMessage>
    {
        private readonly HashSet<int> _fundModels = new HashSet<int>()
        {
            FundModelConstants.CommunityLearning,
            FundModelConstants.AdultSkills,
            FundModelConstants.ESF,
            FundModelConstants.OtherAdult
        };

        private readonly HashSet<int?> _progTypes = new HashSet<int?>()
        {
            TypeOfLearningProgramme.Traineeship,
            TypeOfLearningProgramme.ApprenticeshipStandard
        };

        private readonly IFileDataService _fileDataService;

        public R108Rule(
            IValidationErrorHandler validationErrorHandler,
            IFileDataService fileDataService)
            : base(validationErrorHandler, RuleNameConstants.R108)
        {
            _fileDataService = fileDataService;
        }

        public void Validate(IMessage message)
        {
            if (message.Learners != null)
            {
                foreach (ILearner learner in message.Learners)
                {
                    DateTime? learnActEndDateLatest = null;
                    string ldapLearnRefNumber = string.Empty;

                    if (learner?.LearningDeliveries == null
                        || !FundModelConditionMet(learner.LearningDeliveries)
                        || !AllAimsClosedConditionMet(learner.LearningDeliveries)
                        || !CompStatusConditionMet(learner.LearningDeliveries, out learnActEndDateLatest)
                        || (!learnActEndDateLatest.HasValue
                            || (!FilePreparationDateConditionMet(learnActEndDateLatest.Value)
                                || !DPOutComeConditionMet(
                                    learner.LearnRefNumber,
                                    message?.LearnerDestinationAndProgressions,
                                    learnActEndDateLatest.Value,
                                    out ldapLearnRefNumber))))
                    {
                        continue;
                    }

                    foreach (var learningDelivery in learner.LearningDeliveries)
                    {
                        if (ProgTypeConditionMet(learningDelivery.ProgTypeNullable))
                        {
                            HandleValidationError(
                                learner.LearnRefNumber,
                                learningDelivery.AimSeqNumber,
                                BuildErrorMessageParameters(
                                learningDelivery.FundModel,
                                learningDelivery.CompStatus,
                                learningDelivery.LearnActEndDateNullable,
                                ldapLearnRefNumber));
                        }
                    }
                }
            }
        }

        public bool ProgTypeConditionMet(int? progTypeNullable) => !_progTypes.Contains(progTypeNullable);

        public bool FundModelConditionMet(IReadOnlyCollection<ILearningDelivery> learningDeliveries) =>
            learningDeliveries.Any(ld => _fundModels.Contains(ld.FundModel));

        public bool AllAimsClosedConditionMet(IReadOnlyCollection<ILearningDelivery> learningDeliveries) =>
            learningDeliveries.All(ld => ld.LearnActEndDateNullable.HasValue);

        public bool CompStatusConditionMet(IReadOnlyCollection<ILearningDelivery> learningDeliveries, out DateTime? learnActEndDateLatest)
        {
            bool compStatusCondition = false;
            learnActEndDateLatest = null;

            ILearningDelivery learningDelivery = learningDeliveries
                .Where(ld => ld.LearnActEndDateNullable.HasValue)
                .OrderByDescending(ld => ld.LearnActEndDateNullable)
                .FirstOrDefault();
            if (learningDelivery != null)
            {
                learnActEndDateLatest = learningDelivery.LearnActEndDateNullable;
                compStatusCondition = learningDelivery.CompStatus != CompStatus.LearnerTemporarilyWithdrawn;
            }

            return compStatusCondition;
        }

        public bool FilePreparationDateConditionMet(DateTime learnActEndDateLatest) =>
            _fileDataService.FilePreparationDate() >= learnActEndDateLatest.AddMonths(2);

        public bool DPOutComeConditionMet(
            string learnRefNumber,
            IEnumerable<ILearnerDestinationAndProgression> lDAPs,
            DateTime learnActEndDateLatest,
            out string ldapLearnRefNumber)
        {
            ldapLearnRefNumber = string.Empty;
            bool conditionMet = false;

            var dpOutComesForRefNumber = lDAPs?
                .Where(p => p.LearnRefNumber.CaseInsensitiveEquals(learnRefNumber) && p.DPOutcomes != null)
                .SelectMany(p => p.DPOutcomes).ToList();
            if ((dpOutComesForRefNumber?.Count() ?? 0) == 0)
            {
                conditionMet = true;
            }
            else
            {
                var dpOutCome = dpOutComesForRefNumber
                    .Where(d => d.OutStartDate >= learnActEndDateLatest)?
                    .FirstOrDefault();
                if (dpOutCome?.OutStartDate == null)
                {
                    conditionMet = true;
                    ldapLearnRefNumber = learnRefNumber;
                }
            }

            return conditionMet;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(
            int fundModel,
            int compStatus,
            DateTime? learnActEndDateNullable,
            string ldapLearnRefNumber)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.CompStatus, compStatus),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDateNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearningDestinationAndProgressionLearnRefNumber, ldapLearnRefNumber)
            };
        }
    }
}
