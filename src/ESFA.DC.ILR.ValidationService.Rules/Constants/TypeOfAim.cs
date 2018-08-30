namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    /// <summary>
    /// types of learning aim
    /// </summary>
    public static class TypeOfAim
    {
        /// <summary>
        /// The programme aim
        /// </summary>
        public const int ProgrammeAim = 1;

        /// <summary>
        /// The component aim in a programme
        /// </summary>
        public const int ComponentAimInAProgramme = 3;

        /// <summary>
        /// The aim not part of a programme
        /// </summary>
        public const int AimNotPartOfAProgramme = 4;

        /// <summary>
        /// The core aim 16 to 19 excluding apprenticeships
        /// </summary>
        public const int CoreAim16To19ExcludingApprenticeships = 5;

        /// <summary>
        /// Gets as a set.
        /// </summary>
        public static int[] AsASet => new[]
        {
            ProgrammeAim,
            ComponentAimInAProgramme,
            AimNotPartOfAProgramme,
            CoreAim16To19ExcludingApprenticeships
        };

        public static int[] InAProgramme => new[]
        {
            ProgrammeAim,
            ComponentAimInAProgramme,
        };
    }
}
