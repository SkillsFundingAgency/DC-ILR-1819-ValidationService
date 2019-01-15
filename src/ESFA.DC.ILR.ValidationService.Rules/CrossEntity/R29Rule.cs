using System;
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
    public class R29Rule : AbstractRule, IRule<ILearner>
    {
        public R29Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R29)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            var openComponentAims = objectToValidate.LearningDeliveries.Where(x =>
                x.AimType == TypeOfAim.ComponentAimInAProgramme && !x.LearnActEndDateNullable.HasValue);

            var openProgrammeAims = objectToValidate.LearningDeliveries.Where(x =>
                x.AimType == TypeOfAim.ProgrammeAim && !x.LearnActEndDateNullable.HasValue).ToList();

            foreach (var componentAim in openComponentAims)
            {
                if (ConditionMet(openProgrammeAims, componentAim))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        componentAim.AimSeqNumber,
                        BuildErrorMessageParameters(componentAim));
                }
            }
        }

        public bool ConditionMet(IEnumerable<ILearningDelivery> programmeAims, ILearningDelivery componentAim)
        {
            var isProgrammeAimFound = programmeAims.Any(programmeAim =>
                    programmeAim.FworkCodeNullable == componentAim.FworkCodeNullable &&
                    programmeAim.StdCodeNullable == componentAim.StdCodeNullable &&
                    programmeAim.PwayCodeNullable == componentAim.PwayCodeNullable &&
                    programmeAim.ProgTypeNullable == componentAim.ProgTypeNullable);

            return !isProgrammeAimFound;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(ILearningDelivery componentAim)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, componentAim.AimType),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, componentAim.ProgTypeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.FworkCode, componentAim.FworkCodeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.PwayCode, componentAim.PwayCodeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.StdCode, componentAim.StdCodeNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, componentAim.LearnActEndDateNullable)
            };
        }
    }
}
