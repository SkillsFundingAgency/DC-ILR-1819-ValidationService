using System.Collections.Generic;
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

            var groups = objectToValidate.LearningDeliveries
                .GroupBy(ld => new { ld.AimSeqNumber, ld.ProgTypeNullable, ld.FworkCodeNullable, ld.PwayCodeNullable })
                .Where(grp => grp.Key.ProgTypeNullable != ExcludedProgType);

            foreach (var group in groups)
            {
                if (group.Any(d => d.AimType == ComponentAimType) && group.All(d => d.AimType != ProgramAimType))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        group.Key.AimSeqNumber,
                        BuildErrorMessageParameters(group.Key.ProgTypeNullable, group.Key.FworkCodeNullable, group.Key.PwayCodeNullable));
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