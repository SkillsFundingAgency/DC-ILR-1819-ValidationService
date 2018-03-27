using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth
{
    public class DateOfBirth_06Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IValidationDataService _validationDataService;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        private readonly IEnumerable<long> _fundModels = new HashSet<long>() { 25, 82 };

        public DateOfBirth_06Rule(IValidationDataService validationDataService, IDateTimeQueryService dateTimeQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler)
        {
            _validationDataService = validationDataService;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries != null)
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (ConditionMet(objectToValidate.DateOfBirthNullable, _validationDataService.AcademicYearAugustThirtyFirst, learningDelivery.FundModelNullable))
                    {
                        HandleValidationError(RuleNameConstants.DateOfBirth_06, objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumberNullable);
                    }
                }
            }
        }

        public bool ConditionMet(DateTime? dateOfBirth, DateTime academicYearAugustThirtyFirst, long? fundModel)
        {
            return fundModel.HasValue
                && _fundModels.Contains(fundModel.Value)
                && dateOfBirth.HasValue
                && _dateTimeQueryService.YearsBetween(dateOfBirth.Value, academicYearAugustThirtyFirst) < 13;
        }
    }
}