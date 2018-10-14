using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutCollDate;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules.Abstract;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Modules
{
    public class LearnerDPRuleSetModule : AbstractRuleSetModule
    {
        public LearnerDPRuleSetModule()
        {
            RuleSetType = typeof(IRule<ILearnerDestinationAndProgression>);

            Rules = new List<Type>()
            {
                typeof(OutCollDate_01Rule)
            };
        }
    }
}
