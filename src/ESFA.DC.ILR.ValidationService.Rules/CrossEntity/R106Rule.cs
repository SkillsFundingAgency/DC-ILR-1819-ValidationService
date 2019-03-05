using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R106Rule : AbstractRule, IRule<ILearner>
    {
        private readonly DateTime _augFirst2016 = new DateTime(2016, 8, 1);
        private readonly string _famTypeLSF = LearningDeliveryFAMTypeConstants.LSF;

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public R106Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R106)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            var learningDeliveryFAMs = GetApplicableLearningDeliveryFAMs(objectToValidate);

            var overlappingLearningDeliveryFAMs = _learningDeliveryFAMQueryService.GetOverLappingLearningDeliveryFAMsForType(learningDeliveryFAMs, _famTypeLSF);

            if (overlappingLearningDeliveryFAMs.Any())
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(_famTypeLSF));
            }
        }

        public IEnumerable<ILearningDeliveryFAM> GetApplicableLearningDeliveryFAMs(ILearner objectToValidate)
        {
            return
                objectToValidate
                .LearningDeliveries?
                .Where(l => l.LearningDeliveryFAMs != null)
                .SelectMany(ld => ld.LearningDeliveryFAMs)
                .Where(ldf => ldf.LearnDelFAMDateFromNullable >= _augFirst2016).ToList() ?? new List<ILearningDeliveryFAM>();
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string famType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, famType)
            };
        }
    }
}
