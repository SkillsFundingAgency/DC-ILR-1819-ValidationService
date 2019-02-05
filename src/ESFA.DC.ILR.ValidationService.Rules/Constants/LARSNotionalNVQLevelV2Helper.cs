﻿using ESFA.DC.ILR.ValidationService.Utility;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    /// <summary>
    /// a lars notional nvq level v2 helper class
    /// </summary>
    public static class LARSNotionalNVQLevelV2Helper
    {
        /// <summary>
        /// The notional level v2 map
        /// </summary>
        private static readonly Dictionary<string, TypeOfNotionalNVQLevelV2> _notionalLevelV2Map = new Dictionary<string, TypeOfNotionalNVQLevelV2>
        {
            [LARSNotionalNVQLevelV2.EntryLevel] = TypeOfNotionalNVQLevelV2.EntryLevel,
            [LARSNotionalNVQLevelV2.Level1] = TypeOfNotionalNVQLevelV2.Level1,
            [LARSNotionalNVQLevelV2.Level2] = TypeOfNotionalNVQLevelV2.Level2,
            [LARSNotionalNVQLevelV2.Level3] = TypeOfNotionalNVQLevelV2.Level3,
            [LARSNotionalNVQLevelV2.HigherLevel] = TypeOfNotionalNVQLevelV2.HigherLevel,

            // valid domain values considered 'out of scope'
            [LARSNotionalNVQLevelV2.Level1_2] = TypeOfNotionalNVQLevelV2.OutOfScope,
            [LARSNotionalNVQLevelV2.Level4] = TypeOfNotionalNVQLevelV2.OutOfScope,
            [LARSNotionalNVQLevelV2.Level5] = TypeOfNotionalNVQLevelV2.OutOfScope,
            [LARSNotionalNVQLevelV2.Level6] = TypeOfNotionalNVQLevelV2.OutOfScope,
            [LARSNotionalNVQLevelV2.Level7] = TypeOfNotionalNVQLevelV2.OutOfScope,
            [LARSNotionalNVQLevelV2.Level8] = TypeOfNotionalNVQLevelV2.OutOfScope,
            [LARSNotionalNVQLevelV2.MixedLevel] = TypeOfNotionalNVQLevelV2.OutOfScope,
            [LARSNotionalNVQLevelV2.NotKnown] = TypeOfNotionalNVQLevelV2.OutOfScope
        };

        /// <summary>
        /// As notional NVQ level v2.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>a mapped notional nvq level v2 value</returns>
        public static TypeOfNotionalNVQLevelV2 AsNotionalNVQLevelV2(this string source)
        {
            return It.IsEmpty(source) || !_notionalLevelV2Map.ContainsKey(source)
                ? TypeOfNotionalNVQLevelV2.OutOfScope
                : _notionalLevelV2Map[source];
        }
    }
}
