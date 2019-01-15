using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ULN
{
    public class ULN_10Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IAcademicYearDataService _academicDataQueryService;
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly IFileDataService _fileDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        private readonly IEnumerable<int> _fundModels = new HashSet<int> { 99 };

        public ULN_10Rule(
            IAcademicYearDataService academicDataQueryService,
            IDateTimeQueryService dateTimeQueryService,
            IFileDataService fileDataService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ULN_10)
        {
            _academicDataQueryService = academicDataQueryService;
            _dateTimeQueryService = dateTimeQueryService;
            _fileDataService = fileDataService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            var filePrepDate = _fileDataService.FilePreparationDate();
            var januaryFirst = _academicDataQueryService.JanuaryFirst();

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    objectToValidate.ULN,
                    learningDelivery.FundModel,
                    learningDelivery.LearningDeliveryFAMs,
                    learningDelivery.LearnStartDate,
                    learningDelivery.LearnPlanEndDate,
                    learningDelivery.LearnActEndDateNullable,
                    filePrepDate,
                    januaryFirst))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.ULN, learningDelivery.LearnStartDate, learningDelivery.LearnPlanEndDate, learningDelivery.FundModel, LearningDeliveryFAMTypeConstants.SOF, "1"));
                    return;
                }
            }
        }

        public bool ConditionMet(
            long uln,
            int fundModel,
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs,
            DateTime learnStartDate,
            DateTime learnPlanEndDate,
            DateTime? learnActEndDate,
            DateTime filePrepDate,
            DateTime januaryFirst)
        {
            return UlnConditionMet(uln)
                && FundModelConditionMet(fundModel)
                && FilePreparationDateConditionMet(learnStartDate, filePrepDate, januaryFirst)
                && LearningDatesConditionMet(learnStartDate, learnPlanEndDate, learnActEndDate)
                && LearningDeliveryFAMConditionMet(learningDeliveryFAMs);
        }

        public bool UlnConditionMet(long uln)
        {
            return uln == ValidationConstants.TemporaryULN;
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public bool FilePreparationDateConditionMet(DateTime learnStartDate, DateTime filePrepDate, DateTime januaryFirst)
        {
            return filePrepDate >= januaryFirst
                && _dateTimeQueryService.DaysBetween(learnStartDate, filePrepDate) <= 60;
        }

        public bool LearningDatesConditionMet(DateTime learnStartDate, DateTime learnPlanEndDate, DateTime? learnActEndDate)
        {
            return _dateTimeQueryService.DaysBetween(learnStartDate, learnPlanEndDate) >= 5
                || (learnActEndDate.HasValue && _dateTimeQueryService.DaysBetween(learnStartDate, (DateTime)learnActEndDate) >= 5);
        }

        public virtual bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.LDM, "034")
                && _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.SOF, "1");
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(long uln, DateTime learnStartDate, DateTime learnPlanEndDate, int fundModel, string famType, string famCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ULN, uln),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnPlanEndDate, learnPlanEndDate),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, famType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, famCode)
            };
        }
    }
}