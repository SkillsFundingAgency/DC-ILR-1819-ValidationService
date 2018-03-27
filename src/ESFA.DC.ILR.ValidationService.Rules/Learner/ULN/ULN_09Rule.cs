using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.ExternalData.FileDataService.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ULN
{
    public class ULN_09Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFileDataService _fileDataService;
        private readonly IValidationDataService _validationDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public ULN_09Rule(IFileDataService fileDataService, IValidationDataService validationDataService, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
            _fileDataService = fileDataService;
            _validationDataService = validationDataService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "034"), _fileDataService.FilePreparationDate, _validationDataService.AcademicYearJanuaryFirst, objectToValidate.ULNNullable))
                {
                    HandleValidationError(RuleNameConstants.ULN_09, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
                }
            }
        }

        public bool ConditionMet(bool famTypeAndCodeMatch, DateTime filePreparationDate, DateTime academicYearJanuaryFirst, long? uln)
        {
            return famTypeAndCodeMatch
                && uln == ValidationConstants.TemporaryULN
                && filePreparationDate >= academicYearJanuaryFirst;
        }
    }
}