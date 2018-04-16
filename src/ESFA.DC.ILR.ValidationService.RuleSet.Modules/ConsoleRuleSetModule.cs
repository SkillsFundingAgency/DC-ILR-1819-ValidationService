using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ULN;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AddHours;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AimSeqNumber;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AimType;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.CompStatus;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ConRefNumber;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.DelLocPostCode;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.EmpOutcome;
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
                typeof(AddHours_04Rule),
                typeof(AddHours_05Rule),
                typeof(AddHours_06Rule),
                typeof(AimSeqNumber_02Rule),
                typeof(AimType_01Rule),
                typeof(AimType_05Rule),
                typeof(AimType_07Rule),
                typeof(CompStatus_01Rule),
                typeof(CompStatus_02Rule),
                typeof(CompStatus_03Rule),
                typeof(CompStatus_04Rule),
                typeof(CompStatus_05Rule),
                typeof(CompStatus_06Rule),
                typeof(ConRefNumber_01Rule),
                typeof(ConRefNumber_03Rule),
                typeof(DelLocPostCode_03Rule),
                typeof(DelLocPostCode_11Rule),
                typeof(DelLocPostCode_16Rule),
                typeof(EmpOutcome_01Rule),
                typeof(ULN_03Rule),
            };
        }
    }
}
