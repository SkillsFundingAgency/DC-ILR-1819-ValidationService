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
    public class R69Rule : AbstractRule, IRule<IMessage>
    {
        public R69Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R69)
        {
        }

        public void Validate(IMessage objectToValidate)
        {
            if (objectToValidate?.LearnerDestinationAndProgressions == null)
            {
                return;
            }

            var dpCoutcomes = objectToValidate.LearnerDestinationAndProgressions
                .GroupBy(ldp => new
                {
                    ldp.LearnRefNumber,
                    DpOutComeCount = ldp.DPOutcomes.GroupBy(x => new { x.OutCode, x.OutType, x.OutStartDate }).Select(y => y.Count())
                })
                .Where(x => x.Key.DpOutComeCount.Any(c => c > 1))
                .Select(x => x.Key.LearnRefNumber);

            foreach (var duplicate in dpCoutcomes)
            {
                HandleValidationError(duplicate);
            }
        }
    }
}
