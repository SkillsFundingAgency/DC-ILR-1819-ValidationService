namespace ESFA.DC.ILR.ValidationService.Rules.HE.FinancialSupport.FINTYPE
{
    /// <summary>
    /// types of financial support
    /// </summary>
    public static class TypeOfFinancialSupport
    {
        public const int Cash = 1;
        public const int NearCash = 2;
        public const int AccommodationDiscount = 3;
        public const int Other = 4;

        public static int[] AsASet => new[]
        {
            Cash,
            NearCash,
            AccommodationDiscount,
            Other
        };
    }
}
