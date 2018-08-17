using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Message.UKPRN;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules.Abstract;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Modules
{
    public class MessageRuleSetModule : AbstractRuleSetModule
    {
        public MessageRuleSetModule()
        {
            RuleSetType = typeof(IRule<IMessage>);

            Rules = new List<Type>()
            {
                typeof(UKPRN_03Rule),
            };
        }
    }
}
