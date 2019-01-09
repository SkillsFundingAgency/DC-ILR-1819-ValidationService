namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_14Rule
    {
        char InvalidLengthChecksum { get; }

        char GetWorkPlaceEmpIdChecksum(int workPlaceEmpId);
    }
}
