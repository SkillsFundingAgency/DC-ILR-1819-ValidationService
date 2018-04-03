using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AddHours;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules.Abstract;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Modules
{
    public class ConsoleRuleSetModule : AbstractRuleSetModule
    {
        public ConsoleRuleSetModule()
        {
            RuleSetType = typeof(IRule<ILearner>);

            Rules = new List<Type>()
            {
                typeof(AddHours_01Rule),
                typeof(AddHours_02Rule),
            };
        }
    }
}
