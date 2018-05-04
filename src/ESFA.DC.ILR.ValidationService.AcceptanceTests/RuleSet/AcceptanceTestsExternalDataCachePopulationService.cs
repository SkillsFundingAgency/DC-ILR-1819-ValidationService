using System;
using System.Collections.Generic;
using System.IO;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using Newtonsoft.Json.Linq;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class AcceptanceTestsExternalDataCachePopulationService : IExternalDataCachePopulationService
    {
        private AcceptanceTestsExternalDataCache _dataCache;

        public AcceptanceTestsExternalDataCachePopulationService(IExternalDataCache iCache)
        {
            _dataCache = (AcceptanceTestsExternalDataCache)iCache;
        }

        public void Populate()
        {
            string content = File.ReadAllText(@"Files\AcceptanceTestsReferenceData.json");
            dynamic rhs = JObject.Parse(content);
            _dataCache.ULNs = new List<long>();

            PopulateOrganisations(rhs);
            PopulateFrameworksFrameworkAimsAndLearningDeliveries(rhs);
        }

        private void PopulateFrameworksFrameworkAimsAndLearningDeliveries(dynamic rhs)
        {
            var frameworks = new List<Framework>();
            var learningDeliveries = new Dictionary<string, LearningDelivery>();

            var appaims = rhs._apprenticeShipAims as IEnumerable<dynamic>;
            foreach (var v in appaims)
            {
                int progType = v.Value.ProgType;
                int fworkCode = v.Value.FworkCode;
                int pwayCode = v.Value.PwayCode;
                int stdCode = v.Value.StdCode;
                LearningDelivery ld = BuildLearningDelivery(learningDeliveries, v);

                if (ld.FrameworkCommonComponent == FrameworkCommonComponent.NotApplicable &&
                    fworkCode > 0)
                {
                    BuildFrameworkAimsAndCommonComponents(frameworks, v, progType, fworkCode, pwayCode, ld);
                }
            }

            foreach (var ld in rhs._learningDelivery)
            {
                string aimRef = ld.LearnAimRef;
                int? fcc = ld.FrameworkCommonComponent;
                learningDeliveries.Add(
                    aimRef,
                    new LearningDelivery() { LearnAimRef = aimRef, FrameworkCommonComponent = fcc });
            }

            _dataCache.Frameworks = frameworks;
            _dataCache.LearningDeliveries = learningDeliveries;
        }

        private void BuildFrameworkAimsAndCommonComponents(List<Framework> frameworks, dynamic v, int progType, int fworkCode, int pwayCode, LearningDelivery ld)
        {
            var framework = new Framework()
            {
                ProgType = progType,
                FworkCode = fworkCode,
                PwayCode = pwayCode,
                FrameworkAims = new List<FrameworkAim>()
                {
                    new FrameworkAim()
                    {
                        ProgType = progType,
                        FworkCode = fworkCode,
                        PwayCode = pwayCode,
                        LearnAimRef = ld.LearnAimRef
                    }
                }
            };

            var array = v.Value.FrameworkCommonComponents;
            var lfcc = new List<FrameworkCommonComponent>();

            foreach (var element in array)
            {
                FrameworkCommonComponent fcc = new FrameworkCommonComponent()
                {
                    CommonComponent = element.CommonComponent,
                    ProgType = progType,
                    FworkCode = fworkCode,
                    PwayCode = pwayCode,
                    EffectiveFrom = element.EffectiveFrom,
                    EffectiveTo = element.EffectiveTo
                };
                lfcc.Add(fcc);
            }

            framework.FrameworkCommonComponents = lfcc;
            frameworks.Add(framework);
        }

        private LearningDelivery BuildLearningDelivery(Dictionary<string, LearningDelivery> learningDeliveries, dynamic v)
        {
            LearningDelivery ld = new LearningDelivery()
            {
                LearnAimRef = v.Value.LearningDelivery.LearnAimRef,
                FrameworkCommonComponent = v.Value.LearningDelivery.FrameworkCommonComponent
            };

            learningDeliveries.Add(ld.LearnAimRef, ld);
            return ld;
        }

        private void PopulateOrganisations(dynamic rhs)
        {
            _dataCache.Organisations = new Dictionary<long, Organisation>()
            {
                { rhs["_organisations"]["SpecialistDesignatedCollege"]["UKPRN"].Value, null },
                { rhs["_organisations"]["DummyOrganisationTestingOnly"]["UKPRN"].Value, null },
                { rhs["_organisations"]["PartnerOrganisation"]["UKPRN"].Value, null },
            };
        }
    }
}
