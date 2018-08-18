namespace ESFA.DC.ILR.ValidationService.Rules.HE.TTACCOM
{
    /// <summary>
    /// type of term time accomodation
    /// this type maps to directly integer values
    /// </summary>
    public static class TermTimeAccommodation
    {
        // public const int OwnHome = 3; <= deprecated on the 31/07/2008
        public const int InstitutionMaintainedProperty = 1;
        public const int ParentaOrGuardianHome = 2;
        public const int Other = 4;
        public const int NotKnown = 5;
        public const int NotInAttendanceAtTheInstitution = 6;
        public const int OwnResidence = 7;
        public const int OtherRentedAccommodation = 8;
        public const int PrivateSectorHalls = 9;

        public static int[] AsASet => new[]
        {
            InstitutionMaintainedProperty,
            ParentaOrGuardianHome,
            Other,
            NotKnown,
            NotInAttendanceAtTheInstitution,
            OwnResidence,
            OtherRentedAccommodation,
            PrivateSectorHalls
        };
    }
}
