using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_26Rule
    {
        bool LearnerOnBenefitsAtStartOfCompletedZESF0001AimForContract(ILearner learner, string conRefNumber);
    }
}
