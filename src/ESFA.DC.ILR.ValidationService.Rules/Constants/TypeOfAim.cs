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
            /// esf learner start and assessment
            /// </summary>
            public const string ESFLearnerStartandAssessment = "ZESF0001";

            /// <summary>
            /// Gets the vocational studies not leading to a recognised qualification.
            /// </summary>
            public static string[] VocationalStudiesNotLeadingToARecognisedQualification => new[]
            {
                "ZVOC0001", "ZVOC0002", "ZVOC0003", "ZVOC0004", "ZVOC0005", "ZVOC0006",
                "ZVOC0007", "ZVOC0008", "ZVOC0009", "ZVOC0010", "ZVOC0011", "ZVOC0012",
                "ZVOC0013", "ZVOC0014", "ZVOC0015", "ZUXA103E", "ZUXA105C", "ZUXA107C",
                "ZUXA108B", "ZUXA203E", "ZUXA204A", "ZUXA204C", "ZUXA205C", "ZUXA206B",
                "ZUXA206C", "ZUXA207C", "ZUXA208B", "ZUXA209A", "ZUXA214A", "ZUXA214B",
                "ZUXA215A", "ZUXA301A", "ZUXA301B", "ZUXA302B", "ZUXA303E", "ZUXA304A",
                "ZUXA304C", "ZUXA305A", "ZUXA305C", "ZUXA306B", "ZUXA307C", "ZUXA314B",
                "ZUXA315A", "ZUXAE05C", "ZUXAE06B", "ZUXAE14A", "ZUXAH01B", "ZUXAH09C",
                "ZUXAH15A", "ZUXAH15B"
            };

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
