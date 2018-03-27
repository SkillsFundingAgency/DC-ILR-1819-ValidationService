using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.ExternalData.FileDataService.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ULN
{
    public class ULN_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFileDataService _fileDataService;
        private readonly IValidationDataService _validationDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        private readonly IEnumerable<long?> _fundModels = new long?[] { 25, 82, 35, 36, 81, 70 };

        public ULN_03Rule(IFileDataService fileDataService, IValidationDataService validationDataService, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
            _fileDataService = fileDataService;
            _validationDataService = validationDataService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(ld => !Exclude(ld)))
            {
                if (ConditionMet(learningDelivery.FundModelNullable, objectToValidate.ULNNullable, _fileDataService.FilePreparationDate, _validationDataService.AcademicYearJanuaryFirst))
                {
                    HandleValidationError(RuleNameConstants.ULN_03, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
                }
            }
        }

        public bool ConditionMet(long? fundModel, long? uln, DateTime filePreparationDate, DateTime academicYearJanuaryFirst)
        {
            return _fundModels.Contains(fundModel)
                && uln == ValidationConstants.TemporaryULN
                && filePreparationDate < academicYearJanuaryFirst;
        }

        public bool Exclude(ILearningDelivery learningDelivery)
        {
            return _learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT, "1");
        }
    }
}