namespace ESFA.DC.ILR.ValidationService.Data.Population.Keys
{
    public struct FrameworkKey
    {
        public FrameworkKey(int fworkCode, int progType, int pwayCode)
        {
            FworkCode = fworkCode;
            ProgType = progType;
            PwayCode = pwayCode;
        }

        public int FworkCode { get; }

        public int ProgType { get;  }

        public int PwayCode { get; }
    }
}
