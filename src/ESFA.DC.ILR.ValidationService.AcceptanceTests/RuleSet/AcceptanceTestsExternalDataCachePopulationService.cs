using System.Collections.Generic;
using System.IO;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using Newtonsoft.Json.Linq;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class AcceptanceTestsExternalDataCachePopulationService : IExternalDataCachePopulationService
    {
        AcceptanceTestsExternalDataCache _dataCache;

        public AcceptanceTestsExternalDataCachePopulationService(IExternalDataCache iCache)
        {
            _dataCache = (AcceptanceTestsExternalDataCache)iCache;
        }

        public void Populate()
        {
            string content = File.ReadAllText(@"Files\AcceptanceTestsReferenceData.json");
            dynamic rhs = JObject.Parse(content);
            string intermediateAim = rhs["_apprenticeShipAims"]["IntermediateLevelApprenticeship"]["LearnAimRef"];
            // intermediateAim.Should().Be(lhs._apprenticeShipAims[ProgType.IntermediateLevelApprenticeship].LearnAimRef);
            _dataCache.ULNs = new List<long>();

            PopulateUKPRNs(rhs);
            PopulateFrameworksAndFrameworkAims(rhs);
        }

        private void PopulateFrameworksAndFrameworkAims(dynamic rhs)
        {
            var frameworks = new List<Framework>();
            JObject appaims = rhs["_apprenticeShipAims"];
            foreach (JProperty v in appaims.Properties())
            {
                var s = v.Name + " " + v.Value;
                int ProgType = v.Value["ProgType"].Value<int>();
                int FworkCode = v.Value["FworkCode"].Value<int>();
                int PwayCode = v.Value["PwayCode"].Value<int>();
                string LearnAimRef = v.Value["LearningDelivery"]["LearnAimRef"].ToString();
                //"Validity": null,
                int StdCode = v.Value["StdCode"].Value<int>();

                FrameworkAim aim = new FrameworkAim()
                {
                    ProgType = ProgType,
                    FworkCode = FworkCode,
                    PwayCode = PwayCode,
                    LearnAimRef = LearnAimRef
                };

                frameworks.Add(new Framework()
                {
                    ProgType = ProgType,
                    FworkCode = v.Value["FworkCode"].Value<int>(),
                    PwayCode = PwayCode,
                    FrameworkAims = new List<FrameworkAim>()
                    {
                        aim
                    }
                });
            }

            _dataCache.Frameworks = frameworks;
        }

        private void PopulateUKPRNs(dynamic rhs)
        {
            _dataCache.UKPRNs = new List<long>()
            {
                rhs["_organisations"]["SpecialistDesignatedCollege"]["UKPRN"].Value,
                rhs["_organisations"]["DummyOrganisationTestingOnly"]["UKPRN"].Value,
                rhs["_organisations"]["PartnerOrganisation"]["UKPRN"].Value
            };
        }
    }
}
