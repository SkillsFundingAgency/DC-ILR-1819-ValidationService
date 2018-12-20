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
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            DateTime? learnActEndDatePrevious = null;
            bool firstRecord = true;
            foreach (var learningDelivery in objectToValidate
                .LearningDeliveries.OrderBy(d => d.LearnStartDate))
            {
                if (firstRecord)
                {
                    learnActEndDatePrevious = learningDelivery.LearnActEndDateNullable;
                }

                if (ConditionMet(
                        learningDelivery.AimType,
                        learningDelivery.ProgTypeNullable,
                        learningDelivery.LearnAimRef,
                        learningDelivery.LearnStartDate,
                        learnActEndDatePrevious,
                        firstRecord))
                {
                    if (!firstRecord)
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
                    firstRecord = false;
                }
            }
        }

        public bool ConditionMet(
            int aimType,
            int? progTypeNullable,
            string learnAimRef,
            DateTime learnStartDate,
            DateTime? learnActEndDatePrevious,
            bool firstRecord)
        {
            return ComponentAimTypeConditionMet(aimType)
                && DD07ConditionMet(progTypeNullable)
                && ApprenticeshipStandardsConditionMet(progTypeNullable)
                && LARSConditionMet(learnAimRef)
                && (firstRecord || LearnStartDateConditionMet(learnStartDate, learnActEndDatePrevious, firstRecord));
        }

        public bool LearnStartDateConditionMet(
            DateTime learnStartDate,
            DateTime? learnActEndDatePrevious,
            bool firstRecord)
            => firstRecord ? learnActEndDatePrevious == null
                : (!learnActEndDatePrevious.HasValue || learnStartDate < learnActEndDatePrevious);

        public bool DD07ConditionMet(int? progType) => _dd07.IsApprenticeship(progType);

        public bool ComponentAimTypeConditionMet(int aimType) => aimType == TypeOfAim.ComponentAimInAProgramme;

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
