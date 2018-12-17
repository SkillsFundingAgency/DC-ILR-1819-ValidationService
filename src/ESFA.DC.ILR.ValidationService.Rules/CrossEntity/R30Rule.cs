﻿using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R30Rule : AbstractRule, IRule<ILearner>
    {
        private const int ExcludedProgType = 25;
        private const int ComponentAimType = 3;
        private const int ProgramAimType = 1;

        public R30Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R30)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            var duplicates = objectToValidate.LearningDeliveries
                .GroupBy(ld => new { ld.ProgTypeNullable, ld.FworkCodeNullable, ld.PwayCodeNullable })
                .Where(grp => grp.Count() > 1 && grp.Key.ProgTypeNullable != ExcludedProgType);

            foreach (var duplicate in duplicates)
            {
                if (duplicate.Any(d => d.AimType == ComponentAimType) && duplicate.All(d => d.AimType != ProgramAimType))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        null,
                        BuildErrorMessageParameters(duplicate.Key.ProgTypeNullable, duplicate.Key.FworkCodeNullable, duplicate.Key.PwayCodeNullable));
                }
            }
        }

        private IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? progType, int? fworkCode, int? pwayCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType.ToString()),
                BuildErrorMessageParameter(PropertyNameConstants.FworkCode, fworkCode.ToString()),
                BuildErrorMessageParameter(PropertyNameConstants.PwayCode, pwayCode.ToString()),
            };
        }
    }
}