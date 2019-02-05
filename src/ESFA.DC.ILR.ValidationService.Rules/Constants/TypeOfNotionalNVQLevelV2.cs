namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    /// <summary>
    /// (type of) notional nvq level v2 (see the helper class <see cref="LARSNotionalNVQLevelV2Helper"/>)
    /// </summary>
    public enum TypeOfNotionalNVQLevelV2
    {
        OutOfScope,
        EntryLevel,
        Level1,
        Level2,
        Level3,
        HigherLevel,

        // items not mapped
        // Level1_2,
        // Level4,
        // Level5,
        // Level6,
        // Level7,
        // Level8,
        // MixedLevel,
        // NotKnown
    }
}
