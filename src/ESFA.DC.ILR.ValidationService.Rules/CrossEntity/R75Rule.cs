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
    public class R75Rule : AbstractRule, IRule<ILearner>
    {
        private const int ProgType = 25;
        private const int ProgAimType = 1;
        private const string PmrFinType = "PMR";

        public R75Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R75)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            var learningDeliveryStandardAims =
                objectToValidate.LearningDeliveries
                    .Where(ld => ld.ProgTypeNullable == ProgType && ld.AimType == ProgAimType && ld.StdCodeNullable.HasValue);

            if (ConditionMet(learningDeliveryStandardAims))
            {
                HandleValidationError(objectToValidate.LearnRefNumber);
            }
        }

        public bool ConditionMet(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            if (learningDeliveries == null)
            {
                return false;
            }

            var AFinValues = learningDeliveries
                .GroupBy(ld => new
                {
                    ld.StdCodeNullable,
                    AFinPayments = ld.AppFinRecords?
                        .Where(x => x.AFinType.CaseInsensitiveEquals(PmrFinType) &&
                                    (x.AFinCode == 1 || x.AFinCode == 2))
                        .Sum(y => y.AFinAmount),
                    AFinReimbersement = ld.AppFinRecords?
                        .Where(x => x.AFinType.CaseInsensitiveEquals(PmrFinType) &&
                                    (x.AFinCode == 3))
                        .Sum(y => y.AFinAmount)
                }).Select(x => new
                {
                    x.Key.StdCodeNullable,
                    x.Key.AFinPayments,
                    x.Key.AFinReimbersement
                });

            return AFinValues.Any(x => x.AFinPayments - x.AFinReimbersement < 0);
        }
    }
}
