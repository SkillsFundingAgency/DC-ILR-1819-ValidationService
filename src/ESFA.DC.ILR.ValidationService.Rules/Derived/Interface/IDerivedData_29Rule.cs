using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    /// <summary>
    /// derived data rule 21
    /// Adult skills funded unemployed learner on other state benefits on learning aim start date
    /// </summary>
    public interface IDerivedData_29Rule
    {
        bool IsInflexibleElementOfTrainingAim(ILearner candidate);
    }
}
