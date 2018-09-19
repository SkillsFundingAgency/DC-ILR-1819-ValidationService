using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Data.LARS.Model;
using ESFA.DC.Data.LARS.Model.Interfaces;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Keys;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public class LARSFrameworkDataRetrievalService : AbstractLARSDataRetrievalService, ILARSFrameworkDataRetrievalService
    {
        private readonly ILARS _lars;

        public LARSFrameworkDataRetrievalService(ILARS lars, ICache<IMessage> messagerCache)
            : base(messagerCache)
        {
            _lars = lars;
        }

        public async Task<List<Framework>> RetrieveAsync(CancellationToken cancellationToken)
        {
            var learnAimRefs = UniqueLearnAimRefsFromMessage(_messageCache.Item).ToList();
            var uniqueFrameworks = UniqueFrameworksFromMessage(_messageCache.Item).ToList();

            var condition = PredicateBuilder.Create<LARS_Framework>(x => false);

            foreach (var frameworkKey in uniqueFrameworks)
            {
                condition = condition
                    .Or(fw =>
                        fw.ProgType == frameworkKey.ProgType
                        && fw.FworkCode == frameworkKey.FworkCode
                        && fw.PwayCode == frameworkKey.PwayCode);
            }

            return await _lars.LARS_Framework.Where(condition)
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
                }).ToListAsync(cancellationToken);
        }

        public IEnumerable<FrameworkKey> UniqueFrameworksFromMessage(IMessage message)
        {
            return message?
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
    }
}
