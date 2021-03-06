﻿using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutCollDate;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutEndDate;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutStartDate;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutType;
using ESFA.DC.ILR.ValidationService.Rules.DestinationAndProgression.OutULN;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules.Abstract;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Modules
{
    public class LearnerDPRuleSetModule : AbstractRuleSetModule
    {
        public LearnerDPRuleSetModule()
        {
            RuleSetType = typeof(IRule<ILearnerDestinationAndProgression>);

            Rules = new List<Type>
            {
                typeof(OutCollDate_01Rule),
                typeof(OutCollDate_02Rule),
                typeof(OutEndDate_01Rule),
                typeof(OutStartDate_01Rule),
                typeof(OutStartDate_02Rule),
                typeof(OutType_01Rule),
                typeof(OutType_02Rule),
                typeof(OutType_03Rule),
                typeof(OutType_04Rule),
                typeof(OutType_05Rule),
                typeof(OutULN_01Rule),
                typeof(OutULN_02Rule)
            };
        }
    }
}
