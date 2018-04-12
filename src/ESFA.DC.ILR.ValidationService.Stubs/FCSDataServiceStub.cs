using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class FCSDataServiceStub : IFCSDataService
    {
        public bool ConRefNumberExists(string conRefNumber)
        {
            return false;
        }
    }
}
