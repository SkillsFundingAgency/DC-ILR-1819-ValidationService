using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ULN
{
    public class ULN_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFileDataCache _fileDataCache;
        private readonly IAcademicYearDataService _academicYearDataService;
        private readonly ILearnerQueryService _learnerQueryService;

        private readonly IEnumerable<long> _fundModels =
            new HashSet<long>()
            {
                FundModelConstants.CommunityLearning,
                FundModelConstants.SixteenToNineteen,
                FundModelConstants.AdultSkills,
                FundModelConstants.Apprenticeships,
                FundModelConstants.OtherAdult,
                FundModelConstants.ESF,
            };

        public ULN_03Rule(IFileDataCache fileDataCache, IAcademicYearDataService academicYearDataService, ILearnerQueryService learnerQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ULN_03)
        {
            _fileDataCache = fileDataCache;
            _academicYearDataService = academicYearDataService;
            _learnerQueryService = learnerQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FundModel, objectToValidate.ULN, _fileDataCache.FilePreparationDate, _academicYearDataService.JanuaryFirst()) && LearningDeliveryFAMConditionMet(objectToValidate))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(objectToValidate.ULN, _fileDataCache.FilePreparationDate));
                    return;
                }
            }
        }

        public bool ConditionMet(long fundModel, long uln, DateTime filePreparationDate, DateTime academicYearJanuaryFirst)
        {
            return uln == ValidationConstants.TemporaryULN
                   && filePreparationDate < academicYearJanuaryFirst
                   && _fundModels.Contains(fundModel);
        }

        public bool LearningDeliveryFAMConditionMet(ILearner learner)
        {
            return !_learnerQueryService.HasLearningDeliveryFAMCodeForType(learner, LearningDeliveryFAMTypeConstants.ACT, "1");
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(long uln, DateTime filePreparationDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ULN, uln),
                BuildErrorMessageParameter(PropertyNameConstants.FilePreparationDate, filePreparationDate.ToString("d", new CultureInfo("en-GB"))),
            };
        }
    }
}