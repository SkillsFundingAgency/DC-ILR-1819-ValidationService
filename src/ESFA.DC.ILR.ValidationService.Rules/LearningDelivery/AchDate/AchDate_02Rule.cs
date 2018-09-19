using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AchDate
{
    public class AchDate_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IAcademicYearDataService _academicYearDataService;

        public AchDate_02Rule(IAcademicYearDataService academicYearDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AchDate_02)
        {
            _academicYearDataService = academicYearDataService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.AchDateNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.AchDateNullable));
                }
            }
        }

        public bool ConditionMet(DateTime? achDate)
        {
            return achDate.HasValue && achDate.Value > _academicYearDataService.End();
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime? achDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AchDate, achDate)
            };
        }
    }
}
