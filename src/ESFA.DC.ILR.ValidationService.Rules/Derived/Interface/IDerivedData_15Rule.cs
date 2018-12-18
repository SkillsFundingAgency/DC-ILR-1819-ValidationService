namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_15Rule
    {
        string Derive(long uln);

        int CalculateCheckSum(long uln);
    }
}