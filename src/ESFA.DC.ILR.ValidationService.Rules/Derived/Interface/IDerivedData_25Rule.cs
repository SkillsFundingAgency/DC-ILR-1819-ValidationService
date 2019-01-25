using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_25Rule
    {
        int? GetLengthOfUnemployment(
            ILearner learner,
            string conRefNumber);
    }
}
