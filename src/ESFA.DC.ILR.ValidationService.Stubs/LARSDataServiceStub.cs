using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class LARSDataServiceStub : ILARSDataService
    {
        public bool FrameworkCodeExistsForCommonComponent(string learnAimRef, int? progType, int? fworkCode, int? pwayCode)
        {
            return true;
        }

        public bool FrameworkCodeExistsForFrameworkAims(string learnAimRef, int? progType, int? fworkCode, int? pwayCode)
        {
            return true;
        }

        public bool LearnAimRefExists(string learnAimRef)
        {
            return true;
        }

        public bool NotionalNVQLevelV2MatchForLearnAimRef(string learnAimRef, string level)
        {
            return false;
        }
    }
}
