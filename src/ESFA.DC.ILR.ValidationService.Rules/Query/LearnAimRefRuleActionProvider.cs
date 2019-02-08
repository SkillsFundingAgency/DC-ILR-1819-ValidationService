using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    public class LearnAimRefRuleActionProvider :
        IProvideLearnAimRefRuleActions
    {
        /// <summary>
        /// The branch actions
        /// </summary>
        private readonly ICollection<Func<ILearningDelivery, ILearner, BranchResult>> _branchActions;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnAimRefRuleActionProvider " /> class.
        /// </summary>
        /// <param name="commonOperations">The common operations.</param>
        /// <param name="larsData">The lars data.</param>
        /// <param name="derivedData11">The derived data 11 (rule).</param>
        public LearnAimRefRuleActionProvider(
            IProvideRuleCommonOperations commonOperations,
            ILARSDataService larsData,
            IDerivedData_11Rule derivedData11)
        {
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));
            It.IsNull(larsData)
                .AsGuard<ArgumentNullException>(nameof(larsData));
            It.IsNull(derivedData11)
                .AsGuard<ArgumentNullException>(nameof(derivedData11));

            Check = commonOperations;
            LarsData = larsData;
            DerivedData11 = derivedData11;

            _branchActions = new List<Func<ILearningDelivery, ILearner, BranchResult>>
            {
                IsQualifyingCategoryAdultSkills,
                IsQualifyingCategoryApprenticeship,
                IsQualifyingCategoryUnemployed,
                IsQualifyingCategory16To19EFA,
                IsQualifyingCategoryCommunityLearning,
                IsQualifyingCategoryOLASS,
                IsQualifyingCategoryAdvancedLearnerLoan,
                IsQualifyingCategoryAny,
                IsQualifyingCategoryESF
            };
        }

        /// <summary>
        /// Gets the apprenticeship minimum start.
        /// </summary>
        public static DateTime ApprenticeshipMinimumStart => new DateTime(2011, 08, 01);

        /// <summary>
        /// Gets the unemployed maximum start.
        /// </summary>
        public static DateTime UnemployedMaximumStart => new DateTime(2016, 08, 01);

        /// <summary>
        /// Gets the (common operations provider) check.
        /// </summary>
        protected IProvideRuleCommonOperations Check { get; }

        /// <summary>
        /// Gets the lars data (service)
        /// </summary>
        protected ILARSDataService LarsData { get; }

        /// <summary>
        /// Gets the derived data 11 (rule)
        /// </summary>
        protected IDerivedData_11Rule DerivedData11 { get; }

        /// <summary>
        /// Determines whether [in receipt of benefits at start] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="employments">The employments.</param>
        /// <returns>
        ///   <c>true</c> if [in receipt of benefits at start] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool InReceiptOfBenefitsAtStart(ILearningDelivery delivery, IReadOnlyCollection<ILearnerEmploymentStatus> employments) =>
            DerivedData11.IsAdultFundedOnBenefitsAtStartOfAim(delivery, employments);

        /// <summary>
        /// Determines whether [has qualifying category] [the specified validity].
        /// </summary>
        /// <param name="validity">The validity.</param>
        /// <param name="desiredCategories">The desired categories.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying category] [the specified validity]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingCategory(ILARSLearningDeliveryValidity validity, params string[] desiredCategories) =>
            It.IsInRange(validity.ValidityCategory, desiredCategories);

        /// <summary>
        /// Determines whether [has qualifying category] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="desiredCategories">The desired categories.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying category] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingCategory(ILearningDelivery delivery, params string[] desiredCategories)
        {
            var validities = LarsData.GetValiditiesFor(delivery.LearnAimRef).AsSafeReadOnlyList();

            return validities
                .Any(x => HasQualifyingCategory(x, desiredCategories));
        }

        /// <summary>
        /// Determines whether [is valid adult skills (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid adult skills (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsQualifyingCategoryAdultSkills(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                Check.HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills)
                    && HasQualifyingCategory(delivery, TypeOfLARSValidity.AdultSkills)
                    && !Check.IsRestart(delivery)
                    && !Check.InApprenticeship(delivery)
                    && !Check.IsLearnerInCustody(delivery),
                TypeOfLARSValidity.AdultSkills);

        /// <summary>
        /// Determines whether [is valid apprenticeship (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid apprenticeship (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsQualifyingCategoryApprenticeship(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                Check.HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills, TypeOfFunding.ApprenticeshipsFrom1May2017)
                    && HasQualifyingCategory(delivery, TypeOfLARSValidity.Apprenticeships)
                    && !Check.IsRestart(delivery)
                    && !Check.IsStandardApprencticeship(delivery)
                    && Check.InApprenticeship(delivery)
                    && Check.IsComponentOfAProgram(delivery)
                    && Check.HasQualifyingStart(delivery, ApprenticeshipMinimumStart, DateTime.Today),
                TypeOfLARSValidity.Apprenticeships);

        /// <summary>
        /// Determines whether [is valid unemployed (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid unemployed (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsQualifyingCategoryUnemployed(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                Check.HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills)
                    && HasQualifyingCategory(delivery, TypeOfLARSValidity.Unemployed)
                    && !Check.IsRestart(delivery)
                    && !Check.InApprenticeship(delivery)
                    && !Check.IsLearnerInCustody(delivery)
                    && Check.HasQualifyingStart(delivery, DateTime.MinValue, UnemployedMaximumStart)
                    && InReceiptOfBenefitsAtStart(delivery, learner?.LearnerEmploymentStatuses.AsSafeReadOnlyList()),
                TypeOfLARSValidity.Unemployed);

        /// <summary>
        /// Determines whether [is valid 16 to 19 efa (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid 16 to 19 efa (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsQualifyingCategory16To19EFA(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                Check.HasQualifyingFunding(delivery, TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfFunding.Other16To19)
                    && HasQualifyingCategory(delivery, TypeOfLARSValidity.EFA16To19)
                    && !Check.IsRestart(delivery),
                TypeOfLARSValidity.EFA16To19);

        /// <summary>
        /// Determines whether [is valid community learning (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid community learning (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsQualifyingCategoryCommunityLearning(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                Check.HasQualifyingFunding(delivery, TypeOfFunding.CommunityLearning)
                    && HasQualifyingCategory(delivery, TypeOfLARSValidity.CommunityLearning)
                    && !Check.IsRestart(delivery),
                TypeOfLARSValidity.CommunityLearning);

        /// <summary>
        /// Determines whether [is valid olass (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid olass (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsQualifyingCategoryOLASS(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                Check.HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills)
                    && HasQualifyingCategory(delivery, TypeOfLARSValidity.OLASSAdult)
                    && !Check.IsRestart(delivery)
                    && Check.IsLearnerInCustody(delivery),
                TypeOfLARSValidity.OLASSAdult);

        /// <summary>
        /// Determines whether [is valid advanced learner loan (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid advanced learner loan] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsQualifyingCategoryAdvancedLearnerLoan(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                Check.HasQualifyingFunding(delivery, TypeOfFunding.NotFundedByESFA)
                    && HasQualifyingCategory(delivery, TypeOfLARSValidity.AdvancedLearnerLoan)
                    && !Check.IsRestart(delivery)
                    && Check.IsAdvancedLearnerLoan(delivery),
                TypeOfLARSValidity.AdvancedLearnerLoan);

        /// <summary>
        /// Determines whether [is valid any (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid any (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsQualifyingCategoryAny(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                (Check.HasQualifyingFunding(delivery, TypeOfFunding.NotFundedByESFA, TypeOfFunding.OtherAdult)
                        || (Check.HasQualifyingFunding(delivery, TypeOfFunding.ApprenticeshipsFrom1May2017)
                            && Check.IsStandardApprencticeship(delivery)))
                    && HasQualifyingCategory(delivery, TypeOfLARSValidity.Any)
                    && !Check.IsRestart(delivery)
                    && !Check.IsAdvancedLearnerLoan(delivery),
                TypeOfLARSValidity.Any);

        /// <summary>
        /// Determines whether [is valid esf (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid esf (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsQualifyingCategoryESF(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                Check.HasQualifyingFunding(delivery, TypeOfFunding.EuropeanSocialFund)
                    && HasQualifyingCategory(delivery, TypeOfLARSValidity.EuropeanSocialFund)
                    && !Check.IsRestart(delivery),
                TypeOfLARSValidity.EuropeanSocialFund);

        /// <summary>
        /// Gets the branching result for (this delivery and learner)
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="andLearner">and learner.</param>
        /// <returns>
        /// a branching result
        /// </returns>
        public IBranchResult GetBranchingResultFor(ILearningDelivery thisDelivery, ILearner andLearner)
        {
            It.IsNull(thisDelivery)
                .AsGuard<ArgumentNullException>(nameof(thisDelivery));
            It.IsNull(andLearner)
                .AsGuard<ArgumentNullException>(nameof(andLearner));

            foreach (var doActionFor in _branchActions)
            {
                var branch = doActionFor(thisDelivery, andLearner);
                if (branch.Passed)
                {
                    return branch;
                }
            }

            return BranchResult.Create(false, string.Empty);
        }

        public class BranchResult : IBranchResult
        {
            public BranchResult(bool result, string category)
            {
                Passed = result;
                Category = category;
            }

            /// <summary>
            /// Gets a value indicating whether [out of scope].
            /// </summary>
            public bool OutOfScope => !Passed && It.IsEmpty(Category);

            /// <summary>
            /// Gets the category.
            /// </summary>
            public string Category { get; }

            /// <summary>
            /// Gets a value indicating whether this <see cref="BranchResult"/> is passed.
            /// </summary>
            public bool Passed { get; }

            /// <summary>
            /// Creates the specified result.
            /// </summary>
            /// <param name="filterResult">if set to <c>true</c> [filter result].</param>
            /// <param name="category">The category.</param>
            /// <returns>a new branch result</returns>
            public static BranchResult Create(bool filterResult, string category)
                => new BranchResult(filterResult, category);
        }
    }
}
