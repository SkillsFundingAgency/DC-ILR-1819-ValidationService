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

        /// <summary>
        /// Gets the "in a programme" aims.
        /// </summary>
        public static int[] InAProgramme => new[]
        {
            ProgrammeAim,
            ComponentAimInAProgramme,
        };

        /// <summary>
        /// type of aim references
        /// </summary>
        public static class References
        {
            /// <summary>
            /// work placement 0 to 49 hours
            /// </summary>
            public const string WorkPlacement0To49Hours = "Z0007834";

            /// <summary>
            /// work placement 50 to 99 hours
            /// </summary>
            public const string WorkPlacement50To99Hours = "Z0007835";

            /// <summary>
            /// work placement 100 to 199 hours
            /// </summary>
            public const string WorkPlacement100To199Hours = "Z0007836";

            /// <summary>
            /// work placement 200 to 499 hours
            /// </summary>
            public const string WorkPlacement200To499Hours = "Z0007837";

            /// <summary>
            /// work placement 500 plus hours
            /// </summary>
            public const string WorkPlacement500PlusHours = "Z0007838";

            /// <summary>
            /// supported internship 16 to 19
            /// </summary>
            public const string SupportedInternship16To19 = "Z0002347";

            /// <summary>
            /// work experience code
            /// </summary>
            public const string WorkExperience = "ZWRKX001";

            /// <summary>
            /// industry placement code
            /// </summary>
            public const string IndustryPlacement = "ZWRKX002";

            /// <summary>
            /// Gets the work placement codes.
            /// </summary>
            public static string[] AsWorkPlacementCodes => new[]
            {
                WorkPlacement0To49Hours,
                WorkPlacement50To99Hours,
                WorkPlacement100To199Hours,
                WorkPlacement200To499Hours,
                WorkPlacement500PlusHours,
                SupportedInternship16To19,
                WorkExperience,
                IndustryPlacement,
            };
        }
    }
}
