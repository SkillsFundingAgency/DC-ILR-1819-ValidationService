using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R20Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int?> _frameWorkComponentTypes = new HashSet<int?>() { 1, 3 };

        private readonly ILARSDataService _lARSDataService;
        private readonly IDD07 _dd07;

        public R20Rule(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService lARSDataService,
            IDD07 dd07)
            : base(validationErrorHandler, RuleNameConstants.R20)
        {
            _lARSDataService = lARSDataService;
            _dd07 = dd07;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null
                || (GetLearningDeliveriesForCompetencyAim(objectToValidate.LearningDeliveries)?.Count() ?? 0) <= 1)
            {
                return;
            }

            DateTime? learnActEndDatePrevious = null;
            int recordNo = 1;
            foreach (var learningDelivery in GetLearningDeliveriesForCompetencyAim(
                objectToValidate.LearningDeliveries))
            {
                if (ConditionMet(
                        learningDelivery.ProgTypeNullable,
                        learningDelivery.LearnAimRef,
                        learningDelivery.LearnStartDate,
                        learnActEndDatePrevious,
                        recordNo))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learningDelivery.AimType,
                            learningDelivery.LearnStartDate,
                            learningDelivery.LearnActEndDateNullable));
                }

                learnActEndDatePrevious = learningDelivery.LearnActEndDateNullable;
                recordNo++;
            }
        }

        public bool ConditionMet(
            int? progTypeNullable,
            string learnAimRef,
            DateTime learnStartDate,
            DateTime? learnActEndDatePrevious,
            int recordNo)
        {
            return ApprenticeshipStandardsConditionMet(progTypeNullable)
                && LARSConditionMet(learnAimRef)
                && LearnStartDateConditionMet(learnStartDate, learnActEndDatePrevious, recordNo);
        }

        public IReadOnlyCollection<ILearningDelivery> GetLearningDeliveriesForCompetencyAim(IReadOnlyCollection<ILearningDelivery> learningDeliveries)
            => learningDeliveries?
            .Where(d => d.AimType == TypeOfAim.ComponentAimInAProgramme
                && _dd07.IsApprenticeship(d.ProgTypeNullable))
            .OrderBy(d => d.LearnStartDate)
            .ToList();

        public bool LearnStartDateConditionMet(
            DateTime learnStartDate,
            DateTime? learnActEndDatePrevious,
            int recordNo)
            => recordNo == 1 ? learnActEndDatePrevious == null : learnStartDate < learnActEndDatePrevious;

        public bool LARSConditionMet(string learnAimRef) => _lARSDataService.FrameWorkComponentTypeExistsInFrameworkAims(learnAimRef, _frameWorkComponentTypes);

        public bool ApprenticeshipStandardsConditionMet(int? progTypeNullable) => progTypeNullable.HasValue
                && progTypeNullable != TypeOfLearningProgramme.ApprenticeshipStandard;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int aimType, DateTime learnStartDate, DateTime? learnActEndDateNullable)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDateNullable)
            };
        }
    }
}
