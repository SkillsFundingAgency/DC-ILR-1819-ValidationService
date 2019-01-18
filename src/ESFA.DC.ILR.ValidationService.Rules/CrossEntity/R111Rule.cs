using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R111Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;
        private readonly HashSet<int> _employmentStatuses = new HashSet<int>()
        {
            TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable,
            TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable
        };

        public R111Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.R111)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null
                || !LearnerEmploymentStatusConditionMet(objectToValidate.LearnerEmploymentStatuses))
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                int? empStat;
                DateTime? dateEmpStatApp;
                DateTime? learnDelFAMDateFrom;
                DateTime? learnDelFAMDateTo;

                if (FundModelConditionMet(learningDelivery.FundModel)
                    && LearningDeliveryFAMsConditionMet(learningDelivery.LearningDeliveryFAMs)
                    && LearnerEmploymentDuringLearningDeliveryConditionMet(
                        objectToValidate.LearnerEmploymentStatuses,
                        learningDelivery.LearningDeliveryFAMs,
                        out dateEmpStatApp,
                        out empStat,
                        out learnDelFAMDateFrom,
                        out learnDelFAMDateTo))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            empStat,
                            dateEmpStatApp,
                            learningDelivery.FundModel,
                            LearningDeliveryFAMTypeConstants.ACT,
                            LearningDeliveryFAMCodeConstants.ACT_ContractEmployer,
                            learnDelFAMDateFrom,
                            learnDelFAMDateTo));
                }
            }
        }

        public bool LearnerEmploymentDuringLearningDeliveryConditionMet(
            IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmploymentStatuses,
            IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs,
            out DateTime? empStartDateApp,
            out int? empStat,
            out DateTime? learnDelFAMDateFrom,
            out DateTime? learnDelFAMDateTo)
        {
            var learnerEmployment = (from emp in learnerEmploymentStatuses
                                      from del in learningDeliveryFAMs
                                      where emp.DateEmpStatApp >= del.LearnDelFAMDateFromNullable
                                          && (del.LearnDelFAMDateToNullable.HasValue || emp.DateEmpStatApp <= del.LearnDelFAMDateToNullable)
                                          && del.LearnDelFAMType == LearningDeliveryFAMTypeConstants.ACT
                                          && del.LearnDelFAMCode == LearningDeliveryFAMCodeConstants.ACT_ContractEmployer
                                      select new
                                      {
                                          emp.DateEmpStatApp,
                                          emp.EmpStat,
                                          del.LearnDelFAMDateFromNullable,
                                          del.LearnDelFAMDateToNullable
                                      })?.FirstOrDefault();
            if (learnerEmployment != null)
            {
                empStartDateApp = learnerEmployment.DateEmpStatApp;
                empStat = learnerEmployment.EmpStat;
                learnDelFAMDateFrom = learnerEmployment.LearnDelFAMDateFromNullable;
                learnDelFAMDateTo = learnerEmployment.LearnDelFAMDateToNullable;
                return true;
            }

            empStartDateApp = null;
            empStat = null;
            learnDelFAMDateFrom = null;
            learnDelFAMDateTo = null;
            return false;
        }

        public bool LearnerEmploymentStatusConditionMet(IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmploymentStatuses)
            => learnerEmploymentStatuses?.Any(e => _employmentStatuses.Contains(e.EmpStat)) ?? false;

        public bool LearningDeliveryFAMsConditionMet(IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs)
            => _learningDeliveryFAMQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT);

        public bool FundModelConditionMet(int fundModel) => fundModel == TypeOfFunding.ApprenticeshipsFrom1May2017;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(
            int? empStat,
            DateTime? dateEmpStatApp,
            int fundModel,
            string learnDelFAMType,
            string learnDelFAMCode,
            DateTime? learnDelFAMDateFromNullable,
            DateTime? learnDelFAMDateToNullable)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.EmpStat, empStat),
                BuildErrorMessageParameter(PropertyNameConstants.DateEmpStatApp, dateEmpStatApp),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learnDelFAMCode),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateFrom, learnDelFAMDateFromNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, learnDelFAMDateToNullable)
            };
        }
    }
}
