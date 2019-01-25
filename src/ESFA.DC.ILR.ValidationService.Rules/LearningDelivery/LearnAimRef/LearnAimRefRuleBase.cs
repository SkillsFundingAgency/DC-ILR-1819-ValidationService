using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public abstract class LearnAimRefRuleBase :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "LearnAimRef";

        private readonly ICollection<Func<ILearningDelivery, ILearner, BranchResult>> _branchActions;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnAimRefRuleBase" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="larsData">The lars data.</param>
        /// <param name="derivedData07">The derived data 07 (rule).</param>
        /// <param name="derivedData11">The derived data 11 (rule).</param>
        protected LearnAimRefRuleBase(
            IValidationErrorHandler validationErrorHandler,
            ILARSDataService larsData,
            IDerivedData_07Rule derivedData07,
            IDerivedData_11Rule derivedData11)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(larsData)
                .AsGuard<ArgumentNullException>(nameof(larsData));
            It.IsNull(derivedData07)
                .AsGuard<ArgumentNullException>(nameof(derivedData07));
            It.IsNull(derivedData11)
                .AsGuard<ArgumentNullException>(nameof(derivedData11));

            MessageHandler = validationErrorHandler;
            LarsData = larsData;
            DerivedData07 = derivedData07;
            DerivedData11 = derivedData11;

            _branchActions = new List<Func<ILearningDelivery, ILearner, BranchResult>>
            {
                IsValidAdultSkills,
                IsValidApprenticeship,
                IsValidUnemployed,
                IsValid16To19EFA,
                IsValidCommunityLearning,
                IsValidOLASS,
                IsValidAdvancedLearnerLoan,
                IsValidAny,
                IsValidESF
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
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => GetName();

        /// <summary>
        /// Gets the message handler
        /// </summary>
        protected IValidationErrorHandler MessageHandler { get; }

        /// <summary>
        /// Gets the lars data (service)
        /// </summary>
        protected ILARSDataService LarsData { get; }

        /// <summary>
        /// Gets the derived data 07 (rule)
        /// </summary>
        protected IDerivedData_07Rule DerivedData07 { get; }

        /// <summary>
        /// Gets the derived data 11 (rule)
        /// </summary>
        protected IDerivedData_11Rule DerivedData11 { get; }

        /// <summary>
        /// Gets the (rule) name.
        /// </summary>
        /// <returns>the rule name</returns>
        public abstract string GetName();

        /// <summary>
        /// Checks the delivery fams.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="matchCondition">The match condition.</param>
        /// <returns>true if any of the delivery fams match the condition</returns>
        public bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition) =>
            delivery.LearningDeliveryFAMs.SafeAny(matchCondition);

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
        /// Determines whether [in apprenticeship] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [in apprenticeship] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool InApprenticeship(ILearningDelivery delivery) =>
            DerivedData07.IsApprenticeship(delivery.ProgTypeNullable);

        /// <summary>
        /// Determines whether [in standard apprenticeship] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [in standard apprenticeship] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool InStandardApprenticeship(ILearningDelivery delivery) =>
            It.IsInRange(delivery.ProgTypeNullable, TypeOfLearningProgramme.ApprenticeshipStandard);

        /// <summary>
        /// Determines whether [is component of a program] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is component of a program] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsComponentOfAProgram(ILearningDelivery delivery) =>
            It.IsInRange(delivery.AimType, TypeOfAim.ComponentAimInAProgramme);

        /// <summary>
        /// Determines whether [has qualifying funding] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="desiredFundings">The desired fundings.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying funding] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingFunding(ILearningDelivery delivery, params int[] desiredFundings) =>
            It.IsInRange(delivery.FundModel, desiredFundings);

        /// <summary>
        /// Determines whether [is viable start] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="minStart">The minimum start.</param>
        /// <param name="maxStart">The maximum start.</param>
        /// <returns>
        ///   <c>true</c> if [is viable start] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsViableStart(ILearningDelivery delivery, DateTime minStart, DateTime maxStart) =>
            It.IsBetween(delivery.LearnStartDate, minStart, maxStart);

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
        /// Determines whether [is advanced learner loan] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is advanced learner loan] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAdvancedLearnerLoan(ILearningDeliveryFAM monitor) =>
            It.IsInRange(monitor.LearnDelFAMType, Monitoring.Delivery.Types.AdvancedLearnerLoan);

        /// <summary>
        /// Determines whether [is advanced learner loan] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is advanced learner loan] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAdvancedLearnerLoan(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsAdvancedLearnerLoan);

        /// <summary>
        /// Determines whether the specified monitor is restart.
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if the specified monitor is restart; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRestart(ILearningDeliveryFAM monitor) =>
            It.IsInRange(monitor.LearnDelFAMType, Monitoring.Delivery.Types.Restart);

        /// <summary>
        /// Determines whether the specified delivery is restart.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is restart; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRestart(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsRestart);

        /// <summary>
        /// Determines whether [is learner in custody] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is learner in custody] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLearnerInCustody(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.OLASSOffendersInCustody);

        /// <summary>
        /// Determines whether [is learner in custody] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is learner in custody] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLearnerInCustody(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsLearnerInCustody);

        /// <summary>
        /// Determines whether [is valid adult skills (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid adult skills (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsValidAdultSkills(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills)
                    && HasQualifyingCategory(delivery, TypeOfLARSValidity.AdultSkills)
                    && !IsRestart(delivery)
                    && !InApprenticeship(delivery)
                    && !IsLearnerInCustody(delivery),
                TypeOfLARSValidity.AdultSkills);

        /// <summary>
        /// Determines whether [is valid apprenticeship (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid apprenticeship (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsValidApprenticeship(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills, TypeOfFunding.ApprenticeshipsFrom1May2017)
                    && HasQualifyingCategory(delivery, TypeOfLARSValidity.Apprenticeships)
                    && !IsRestart(delivery)
                    && !InStandardApprenticeship(delivery)
                    && InApprenticeship(delivery)
                    && IsComponentOfAProgram(delivery)
                    && IsViableStart(delivery, ApprenticeshipMinimumStart, DateTime.Today),
                TypeOfLARSValidity.Apprenticeships);

        /// <summary>
        /// Determines whether [is valid unemployed (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid unemployed (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsValidUnemployed(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills)
                    && HasQualifyingCategory(delivery, TypeOfLARSValidity.Unemployed)
                    && !IsRestart(delivery)
                    && !IsLearnerInCustody(delivery)
                    && !InApprenticeship(delivery)
                    && IsViableStart(delivery, DateTime.MinValue, UnemployedMaximumStart)
                    && InReceiptOfBenefitsAtStart(delivery, learner.LearnerEmploymentStatuses),
                TypeOfLARSValidity.Unemployed);

        /// <summary>
        /// Determines whether [is valid 16 to 19 efa (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid 16 to 19 efa (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsValid16To19EFA(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                HasQualifyingFunding(delivery, TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfFunding.Other16To19)
                    && HasQualifyingCategory(delivery, TypeOfLARSValidity.EFA16To19)
                    && !IsRestart(delivery),
                TypeOfLARSValidity.EFA16To19);

        /// <summary>
        /// Determines whether [is valid community learning (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid community learning (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsValidCommunityLearning(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                HasQualifyingFunding(delivery, TypeOfFunding.CommunityLearning)
                    && HasQualifyingCategory(delivery, TypeOfLARSValidity.CommunityLearning)
                    && !IsRestart(delivery),
                TypeOfLARSValidity.CommunityLearning);

        /// <summary>
        /// Determines whether [is valid olass (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid olass (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsValidOLASS(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills)
                    && HasQualifyingCategory(delivery, TypeOfLARSValidity.OLASSAdult)
                    && !IsRestart(delivery)
                    && IsLearnerInCustody(delivery),
                TypeOfLARSValidity.OLASSAdult);

        /// <summary>
        /// Determines whether [is valid advanced learner loan (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid advanced learner loan] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsValidAdvancedLearnerLoan(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                HasQualifyingFunding(delivery, TypeOfFunding.NotFundedByESFA)
                    && HasQualifyingCategory(delivery, TypeOfLARSValidity.AdvancedLearnerLoan)
                    && !IsRestart(delivery)
                    && IsAdvancedLearnerLoan(delivery),
                TypeOfLARSValidity.AdvancedLearnerLoan);

        /// <summary>
        /// Determines whether [is valid any (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid any (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsValidAny(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                (HasQualifyingFunding(delivery, TypeOfFunding.NotFundedByESFA, TypeOfFunding.OtherAdult)
                        || (HasQualifyingFunding(delivery, TypeOfFunding.ApprenticeshipsFrom1May2017)
                            && InStandardApprenticeship(delivery)))
                    && HasQualifyingCategory(delivery, TypeOfLARSValidity.Any)
                    && !IsRestart(delivery)
                    && !IsAdvancedLearnerLoan(delivery),
                TypeOfLARSValidity.Any);

        /// <summary>
        /// Determines whether [is valid esf (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid esf (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public BranchResult IsValidESF(ILearningDelivery delivery, ILearner learner) =>
            BranchResult.Create(
                HasQualifyingFunding(delivery, TypeOfFunding.EuropeanSocialFund)
                    && HasQualifyingCategory(delivery, TypeOfLARSValidity.EuropeanSocialFund)
                    && !IsRestart(delivery),
                TypeOfLARSValidity.EuropeanSocialFund);

        /// <summary>
        /// Passes category restrictions.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        /// true if it does...
        /// </returns>
        public BranchResult GetBranchingResult(ILearningDelivery delivery, ILearner learner)
        {
            foreach (var doAction in _branchActions)
            {
                var result = doAction(delivery, learner);
                if (result.Passed)
                {
                    return result;
                }
            }

            return BranchResult.Create(false, string.Empty);
        }

        /// <summary>
        /// Passes the (rule) conditions.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="branchCategory">The branch category.</param>
        /// <returns>
        /// true if it does...
        /// </returns>
        public abstract bool PassesConditions(ILearningDelivery delivery, BranchResult branchCategory);

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery delivery, ILearner learner) =>
            !PassesConditions(delivery, GetBranchingResult(delivery, learner));

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .ForAny(x => IsNotValid(x, objectToValidate), x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(MessageHandler.BuildErrorMessageParameter(MessagePropertyName, thisDelivery.LearnAimRef));

            MessageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }

        public class BranchResult
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
            internal bool Passed { get; }

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
