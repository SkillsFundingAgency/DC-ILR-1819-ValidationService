using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Message.FileLevel.Entity;
using ESFA.DC.ILR.ValidationService.Rules.Message.FileLevel.Header;
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
                typeof(Entity_1Rule),
                typeof(Header_2Rule),
                typeof(Header_3Rule),
                typeof(UKPRN_03Rule),
//                typeof(R06Rule), needs to move up to global rule set or something similar Hal - removed to test very large file dependency on the file data cache
//                typeof(R85Rule),
//                typeof(R107Rule),
//                typeof(R108Rule),
            };
        }
    }
}
