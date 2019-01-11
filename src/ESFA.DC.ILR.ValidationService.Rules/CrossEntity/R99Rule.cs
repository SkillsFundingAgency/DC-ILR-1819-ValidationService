﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R99Rule : AbstractRule, IRule<ILearner>
    {
        public R99Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R99)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            var mainLearningDeliveries = objectToValidate.LearningDeliveries.Where(ld => ld.AimType == TypeOfAim.ProgrammeAim).ToList();

            foreach (var mainLearningDelivery in mainLearningDeliveries)
            {
                if (mainLearningDeliveries.Any(ld =>
                    ld.AimSeqNumber != mainLearningDelivery.AimSeqNumber &&
                    (
                        (!ld.LearnActEndDateNullable.HasValue && !mainLearningDelivery.LearnActEndDateNullable.HasValue) ||
                        (mainLearningDelivery.LearnStartDate >= ld.LearnStartDate &&
                         ld.LearnActEndDateNullable.HasValue &&
                         mainLearningDelivery.LearnStartDate <= ld.LearnActEndDateNullable))))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, mainLearningDelivery.AimSeqNumber, BuildErrorMessageParameters(mainLearningDelivery));
                }
            }
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(ILearningDelivery learningDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, learningDelivery.AimType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learningDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, learningDelivery.LearnActEndDateNullable)
            };
        }
    }
}
