using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PlanLearnHours
{
    public abstract class PlanLearnHoursTestsBase
    {
        protected ILearner SetupLearner(long? planLearnHours, long? planEepHours, long? fundModel)
        {
            return new TestLearner()
            {
                PlanLearnHoursNullable = planLearnHours,
                PlanEEPHoursNullable = planEepHours,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModelNullable = fundModel,
                    }
                }
            };
        }
    }
}
