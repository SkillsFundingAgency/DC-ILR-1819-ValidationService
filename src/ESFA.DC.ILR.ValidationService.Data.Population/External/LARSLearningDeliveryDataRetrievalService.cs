using ESFA.DC.Data.LARS.Model.Interfaces;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public class LARSLearningDeliveryDataRetrievalService : AbstractLARSDataRetrievalService, ILARSLearningDeliveryDataRetrievalService
    {
        private readonly ILARS _lars;

        public LARSLearningDeliveryDataRetrievalService(ILARS lars, ICache<IMessage> messageCache)
            : base(messageCache)
        {
            _lars = lars;
        }

        public async Task<IReadOnlyDictionary<string, LearningDelivery>> RetrieveAsync(CancellationToken cancellationToken)
        {
            var learnAimRefs = UniqueLearnAimRefsFromMessage(_messageCache.Item).ToCaseInsensitiveHashSet();

            var larsFrameworkAims =
                _lars.LARS_FrameworkAims
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
                })
                 .GroupBy(fa => fa.LearnAimRef)
                 .ToCaseInsensitiveDictionary(k => k.Key, v => v.ToList());

            return await _lars
                .LARS_LearningDelivery
                .Where(ld => learnAimRefs.Contains(ld.LearnAimRef))
                .ToCaseInsensitiveAsyncDictionary(
                    ld => ld.LearnAimRef,
                    ld => new LearningDelivery()
                    {
                        FrameworkCommonComponent = ld.FrameworkCommonComponent,
                        LearnAimRef = ld.LearnAimRef,
                        EffectiveFrom = ld.EffectiveFrom,
                        EffectiveTo = ld.EffectiveTo,
                        LearnAimRefType = ld.LearnAimRefType,
                        EnglPrscID = ld.EnglPrscID,
                        NotionalNVQLevel = ld.NotionalNVQLevel,
                        NotionalNVQLevelv2 = ld.NotionalNVQLevelv2,
                        LearnDirectClassSystemCode1 = new LearnDirectClassSystemCode(ld.LearnDirectClassSystemCode1),
                        LearnDirectClassSystemCode2 = new LearnDirectClassSystemCode(ld.LearnDirectClassSystemCode2),
                        LearnDirectClassSystemCode3 = new LearnDirectClassSystemCode(ld.LearnDirectClassSystemCode3),
                        SectorSubjectAreaTier1 = ld.SectorSubjectAreaTier1,
                        SectorSubjectAreaTier2 = ld.SectorSubjectAreaTier2,
                        AnnualValues = ld.LARS_AnnualValue
                            .Select(av => new AnnualValue()
                            {
                                LearnAimRef = av.LearnAimRef,
                                BasicSkills = av.BasicSkills,
                                BasicSkillsType = av.BasicSkillsType,
                                FullLevel2EntitlementCategory = av.FullLevel2EntitlementCategory,
                                FullLevel3EntitlementCategory = av.FullLevel3EntitlementCategory,
                                FullLevel3Percent = av.FullLevel3Percent,
                                EffectiveFrom = av.EffectiveFrom,
                                EffectiveTo = av.EffectiveTo,
                            }).ToList(),

                        // FrameworkAims Assigned outside of Linq - VSTS #65278
                        FrameworkAims = GetFrameworkAims(ld.LearnAimRef, larsFrameworkAims),
                        Categories = ld.LARS_LearningDeliveryCategory
                            .Select(ldc => new LearningDeliveryCategory()
                            {
                                LearnAimRef = ldc.LearnAimRef,
                                CategoryRef = ldc.CategoryRef,
                                EffectiveFrom = ldc.EffectiveFrom,
                                EffectiveTo = ldc.EffectiveTo,
                            }).ToList(),
                        Validities = ld.LARS_Validity
                            .Select(ldc => new LARSValidity
                            {
                                LearnAimRef = ldc.LearnAimRef,
                                ValidityCategory = ldc.ValidityCategory,
                                StartDate = ldc.StartDate,
                                EndDate = ldc.EndDate,
                                LastNewStartDate = ldc.LastNewStartDate
                            }).ToList(),
                    }, cancellationToken);
        }

        private IReadOnlyCollection<FrameworkAim> GetFrameworkAims(string learnAimRef, Dictionary<string, List<FrameworkAim>> frameworkAimsDictionary)
        {
            frameworkAimsDictionary.TryGetValue(learnAimRef, out var frameworkAims);

            return frameworkAims as IReadOnlyCollection<FrameworkAim>;
        }
    }
}
