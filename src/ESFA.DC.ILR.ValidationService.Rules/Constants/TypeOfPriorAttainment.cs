namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    public static class TypeOfPriorAttainment
    {
        /// <summary>
        /// level 1
        /// </summary>
        public const int Level1 = 1;

        /// <summary>
        /// full level 2
        /// </summary>
        public const int FullLevel2 = 2;

        /// <summary>
        /// full level 3
        /// </summary>
        public const int FullLevel3 = 3;

        /// <summary>
        /// level 4 expired 2013-07-31
        /// </summary>
        public const int Level4Expired20130731 = 4;

        /// <summary>
        /// level 5 and above expired 2013-07-31
        /// </summary>
        public const int Level5AndAboveExpired20130731 = 5;

        /// <summary>
        /// other below level 1
        /// </summary>
        public const int OtherBelowLevel1 = 7;

        /// <summary>
        /// entry level
        /// </summary>
        public const int EntryLevel = 9;

        /// <summary>
        /// level 4
        /// </summary>
        public const int Level4 = 10;

        /// <summary>
        /// level 5
        /// </summary>
        public const int Level5 = 11;

        /// <summary>
        /// level 6
        /// </summary>
        public const int Level6 = 12;

        /// <summary>
        /// level 7 and above
        /// </summary>
        public const int Level7AndAbove = 13;

        /// <summary>
        /// other level not known
        /// </summary>
        public const int OtherLevelNotKnown = 97;

        /// <summary>
        /// not known
        /// </summary>
        public const int NotKnown = 98;

        /// <summary>
        /// not qualified
        /// </summary>
        public const int NotQualified = 99;

        /// <summary>
        /// As higher level achievements
        /// </summary>
        public static readonly int[] AsHigherLevelAchievements = new int[]
        {
            FullLevel2,
            FullLevel3,
            Level4Expired20130731,
            Level5AndAboveExpired20130731,
            Level4,
            Level5,
            Level6,
            Level7AndAbove
        };
    }
}
