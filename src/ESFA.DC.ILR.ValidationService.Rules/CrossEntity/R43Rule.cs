using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R43Rule : AbstractRule, IRule<ILearner>
    {
        public R43Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R43)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearnerEmploymentStatuses == null)
            {
                return;
            }

            var duplicates = objectToValidate.LearnerEmploymentStatuses
                .GroupBy(les => new { les.DateEmpStatApp })
                .Where(grp => grp.Count() > 1);

            foreach (var duplicate in duplicates)
            {
                HandleValidationError(
                    objectToValidate.LearnRefNumber,
                    null,
                    BuildErrorMessageParameters(duplicate.Key.DateEmpStatApp));
            }
        }

        private IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime dateEmpStatApp)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateEmpStatApp, dateEmpStatApp)
            };
        }
    }
}