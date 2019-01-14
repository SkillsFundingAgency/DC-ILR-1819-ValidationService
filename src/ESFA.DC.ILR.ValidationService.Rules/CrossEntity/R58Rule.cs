using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R58Rule : AbstractRule, IRule<ILearner>
    {
        private const int _aimType = 5;

        public R58Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R58)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            var coreAims = objectToValidate.LearningDeliveries?.Where(ld => ld.AimType == _aimType);

            if (DoesNotHaveMultipleCoreAims(coreAims))
            {
                return;
            }

            var learnActEndDate = LearnActEndDateForOverlappingCoreAims(coreAims);

            if (learnActEndDate != null)
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(_aimType, learnActEndDate));
            }
        }

        public bool DoesNotHaveMultipleCoreAims(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return
                learningDeliveries != null
                ? learningDeliveries.Count() < 2
                : true;
        }

        public DateTime? LearnActEndDateForOverlappingCoreAims(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            if (learningDeliveries != null)
            {
                var coreAims = learningDeliveries.OrderBy(ld => ld.LearnStartDate).ToArray();

                var coreAimCount = coreAims.Count();

                var i = 1;

                while (i < coreAimCount)
                {
                    var errorConditionMet =
                        coreAims[i - 1].LearnActEndDateNullable == null
                        ? false
                        : coreAims[i - 1].LearnActEndDateNullable >= coreAims[i].LearnStartDate == true;

                    if (errorConditionMet == true)
                    {
                        return coreAims[i - 1].LearnActEndDateNullable;
                    }

                    i++;
                }

                return null;
            }

            return null;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int aimType, DateTime? learnActEndDate)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learnActEndDate)
            };
        }
    }
}
