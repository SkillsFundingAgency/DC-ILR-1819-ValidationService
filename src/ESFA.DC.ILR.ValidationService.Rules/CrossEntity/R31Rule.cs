using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R31Rule : AbstractRule, IRule<ILearner>
    {
        private const int ComponentAimType = 3;
        private const int ProgramAimType = 1;

        public R31Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R31)
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
                .Where(grp => grp.Count() > 1);

            foreach (var duplicate in duplicates)
            {
                if (duplicate.Any(d => d.AimType == ProgramAimType && d.LearnActEndDateNullable == null) && duplicate.All(d => d.AimType != ComponentAimType))
                {
                    var delivery = duplicate.First(d => d.AimType == ProgramAimType && d.LearnActEndDateNullable == null);
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        null,
                        BuildErrorMessageParameters(delivery));
                }
            }
        }

        private IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(ILearningDelivery delivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, ProgramAimType),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, delivery.ProgTypeNullable.ToString()),
                BuildErrorMessageParameter(PropertyNameConstants.FworkCode, delivery.FworkCodeNullable.ToString()),
                BuildErrorMessageParameter(PropertyNameConstants.PwayCode, delivery.PwayCodeNullable.ToString()),
                BuildErrorMessageParameter(PropertyNameConstants.StdCode, delivery.StdCodeNullable.ToString()),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, delivery.LearnActEndDateNullable.ToString()),
            };
        }
    }
}