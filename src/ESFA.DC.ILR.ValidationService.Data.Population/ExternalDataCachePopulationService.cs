using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Data.LARS.Model;
using ESFA.DC.Data.LARS.Model.Interfaces;
using ESFA.DC.Data.Postcodes.Model.Interfaces;
using ESFA.DC.Data.ULN.Model.Interfaces;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Keys;

namespace ESFA.DC.ILR.ValidationService.Data.Population
{
    public class ExternalDataCachePopulationService : IExternalDataCachePopulationService
    {
        private readonly IExternalDataCache _externalDataCache;
        private readonly ICache<IMessage> _messageCache;
        private readonly ILARS _lars;
        private readonly IULN _uln;
        private readonly IPostcodes _postcodes;

        public ExternalDataCachePopulationService(IExternalDataCache externalDataCache, ICache<IMessage> messageCache, ILARS lars, IULN uln, IPostcodes postcodes)
        {
            _externalDataCache = externalDataCache;
            _messageCache = messageCache;
            _lars = lars;
            _uln = uln;
            _postcodes = postcodes;
        }

        public void Populate()
        {
            var message = _messageCache.Item;

            var learnAimRefs = UniqueLearnAimRefsFromMessage(message).ToList();

            var learningDeliveryDictionary = _lars
                .LARS_LearningDelivery
                .Where(ld => learnAimRefs.Contains(ld.LearnAimRef))
                .ToDictionary(
                    ld => ld.LearnAimRef,
                    ld => new LearningDelivery()
                    {
                        FrameworkCommonComponent = ld.FrameworkCommonComponent,
                        LearnAimRef = ld.LearnAimRef,
                        NotionalNVQLevelv2 = ld.NotionalNVQLevelv2,
                        AnnualValues = ld.LARS_AnnualValue
                            .Select(av => new AnnualValue()
                            {
                                LearnAimRef = av.LearnAimRef,
                                BasicSkills = av.BasicSkills,
                                EffectiveFrom = av.EffectiveFrom,
                                EffectiveTo = av.EffectiveTo,
                            }).ToList(),
                        FrameworkAims = ld.LARS_FrameworkAims
                            .Select(fa => new FrameworkAim()
                            {
                                LearnAimRef = fa.LearnAimRef,
                                FrameworkComponentType = fa.FrameworkComponentType,
                                FworkCode = fa.FworkCode,
                                ProgType = fa.ProgType,
                                PwayCode = fa.PwayCode,
                                EffectiveFrom = fa.EffectiveFrom,
                                EffectiveTo = fa.EffectiveTo
                            }).ToList(),
                        LearningDeliveryCategories = ld.LARS_LearningDeliveryCategory
                            .Select(ldc => new LearningDeliveryCategory()
                            {
                                LearnAimRef = ldc.LearnAimRef,
                                CategoryRef = ldc.CategoryRef,
                                EffectiveFrom = ldc.EffectiveFrom,
                                EffectiveTo = ldc.EffectiveTo,
                            }).ToList(),
                    });

            var uniqueFrameworks = UniqueFrameworksFromMessage(message).ToList();
            
            var condition = PredicateBuilder.Create<LARS_Framework>(x => false);

            foreach (var frameworkKey in uniqueFrameworks)
            {
                condition = condition
                    .Or(fw =>
                        fw.ProgType == frameworkKey.ProgType
                        && fw.FworkCode == frameworkKey.FworkCode
                        && fw.PwayCode == frameworkKey.PwayCode);
            }

            var frameworks = _lars.LARS_Framework.Where(condition)
                .Select(fw => new Framework()
                {
                    FworkCode = fw.FworkCode,
                    PwayCode = fw.PwayCode,
                    ProgType = fw.ProgType,
                    EffectiveFrom = fw.EffectiveFrom,
                    EffectiveTo = fw.EffectiveTo,
                    FrameworkAims = fw.LARS_FrameworkAims
                        .Where(fa => learnAimRefs.Contains(fa.LearnAimRef))
                        .Select(fa => new FrameworkAim()
                        {
                            LearnAimRef = fa.LearnAimRef,
                            FrameworkComponentType = fa.FrameworkComponentType,
                            FworkCode = fa.FworkCode,
                            ProgType = fa.ProgType,
                            PwayCode = fa.PwayCode,
                            EffectiveFrom = fa.EffectiveFrom,
                            EffectiveTo = fa.EffectiveTo
                        }).ToList(),
                    FrameworkCommonComponents = fw.LARS_FrameworkCmnComp
                        .Select(fcc => new FrameworkCommonComponent()
                        {
                            FworkCode = fcc.FworkCode,
                            ProgType = fcc.ProgType,
                            PwayCode = fcc.PwayCode,
                            CommonComponent = fcc.CommonComponent,
                            EffectiveFrom = fcc.EffectiveFrom,
                            EffectiveTo = fcc.EffectiveTo,
                        }).ToList(),
                });

            var uniqueULNs = UniqueULNsFromMessage(message).ToList();

            var ulns = _uln.UniqueLearnerNumbers
                .Where(u => uniqueULNs.Contains(u.ULN))
                .Select(u => u.ULN);

            var uniquePostcodes = UniquePostcodesFromMessage(message).ToList();

            var postcodes = _postcodes.MasterPostcodes
                .Where(p => uniquePostcodes.Contains(p.Postcode))
                .Select(p => p.Postcode);

            var externalDataCache = (ExternalDataCache) _externalDataCache;

            externalDataCache.LearningDeliveries = learningDeliveryDictionary;
            externalDataCache.Frameworks = frameworks.ToList();
            externalDataCache.ULNs = new HashSet<long>(ulns);
            externalDataCache.Postcodes = new HashSet<string>(postcodes);
        }

        public IEnumerable<string> UniqueLearnAimRefsFromMessage(IMessage message)
        {
            return message
                       .Learners?
                       .Where(l => l.LearningDeliveries != null)
                       .SelectMany(l => l.LearningDeliveries)
                       .Where(ld => ld.LearnAimRef != null)
                       .Select(ld => ld.LearnAimRef)
                       .Distinct()
                   ?? new List<string>();
        }

        public IEnumerable<FrameworkKey> UniqueFrameworksFromMessage(IMessage message)
        {
            return message
                .Learners?
                .Where(l => l.LearningDeliveries != null)
                .SelectMany(l => l.LearningDeliveries)
                .Where(ld =>
                    ld.FworkCodeNullable.HasValue
                    && ld.ProgTypeNullable.HasValue
                    && ld.PwayCodeNullable.HasValue)
                .GroupBy(ld =>
                    new
                    {
                        FworkCode = ld.FworkCodeNullable,
                        ProgType = ld.ProgTypeNullable,
                        PwayCode = ld.PwayCodeNullable
                    })
                .Select(g =>
                    new FrameworkKey(g.Key.FworkCode.Value, g.Key.ProgType.Value, g.Key.PwayCode.Value)) 
                   ?? new List<FrameworkKey>();
        }

        public IEnumerable<long> UniqueULNsFromMessage(IMessage message)
        {
            return message
                .Learners?
                .Select(l => l.ULN)
                .Distinct()
                ?? new List<long>();
        }

        public IEnumerable<string> UniquePostcodesFromMessage(IMessage message)
        {
            var learnerPostcodes = message
                                        .Learners?
                                        .Where(l => l.Postcode != null)
                                        .Select(l => l.Postcode)
                                        .Distinct()
                ?? new List<string>();

            var learnerPostcodePriors = message
                                        .Learners?
                                        .Where(l => l.PostcodePrior != null)
                                        .Select(l => l.PostcodePrior)
                                        .Distinct()
                ?? new List<string>();

            var learningDeliveryLocationPostcodes = message
                                        .Learners?
                                        .Where(l => l.LearningDeliveries != null)
                                        .SelectMany(l => l.LearningDeliveries)
                                        .Select(ld => ld.DelLocPostCode)
                                        .Distinct()
                ?? new List<string>();

            return learnerPostcodes
                .Union(learnerPostcodePriors)
                .Union(learningDeliveryLocationPostcodes)
                .Distinct();
        }
    }
}
