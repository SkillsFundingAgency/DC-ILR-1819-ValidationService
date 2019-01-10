using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R110RuleTests : AbstractRuleTests<R110Rule>
    {


        private R110Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new R110Rule(learningDeliveryFamQueryService, validationErrorHandler);
        }
    }
}
