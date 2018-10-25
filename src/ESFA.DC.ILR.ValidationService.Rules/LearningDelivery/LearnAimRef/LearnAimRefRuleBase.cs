﻿using ESFA.DC.ILR.Model.Interface;
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

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The lars data (service)
        /// </summary>
        private readonly ILARSDataService _larsData;

        /// <summary>
        /// The derived data 07 (rule)
        /// </summary>
        private readonly IDD07 _derivedData07;

        /// <summary>
        /// The derived data 11 (rule)
        /// </summary>
        private readonly IDerivedData_11Rule _derivedData11;

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
            IDD07 derivedData07,
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

            _messageHandler = validationErrorHandler;
            _larsData = larsData;
            _derivedData07 = derivedData07;
            _derivedData11 = derivedData11;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => GetName();

        public DateTime ApprenticeshipMinimumStart => new DateTime(2011, 07, 31);

        public DateTime UnemployedMaximumStart => new DateTime(2011, 07, 31);

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
            _derivedData11.IsAdultFundedOnBenefitsAtStartOfAim(delivery, employments);

        /// <summary>
        /// Determines whether [in apprenticeship] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [in apprenticeship] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool InApprenticeship(ILearningDelivery delivery) =>
            _derivedData07.IsApprenticeship(delivery.ProgTypeNullable);

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
        /// Determines whether [has valid learning aim] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="desiredCategories">The desired categories.</param>
        /// <returns>
        ///   <c>true</c> if [has valid learning aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasValidLearningAim(ILearningDelivery delivery, params string[] desiredCategories)
        {
            var validities = _larsData.GetValiditiesFor(delivery.LearnAimRef).AsSafeReadOnlyList();

            return validities
                .Where(x => InValidStartRange(x, delivery))
                .Any(x => HasQualifyingCategory(x, desiredCategories));
        }

        /// <summary>
        /// Determines whether [in valid start range] [the specified validity].
        /// </summary>
        /// <param name="validity">The validity.</param>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [in valid start range] [the specified validity]; otherwise, <c>false</c>.
        /// </returns>
        public bool InValidStartRange(ILARSValidity validity, ILearningDelivery delivery) =>
            It.IsBetween(delivery.LearnStartDate, validity.StartDate, validity.LastNewStartDate ?? DateTime.MaxValue);

        /// <summary>
        /// Determines whether [has qualifying category] [the specified validity].
        /// </summary>
        /// <param name="validity">The validity.</param>
        /// <param name="desiredCategories">The desired categories.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying category] [the specified validity]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingCategory(ILARSValidity validity, params string[] desiredCategories) =>
            It.IsInRange(validity.ValidityCategory, desiredCategories);

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
            CheckDeliveryFAMs(delivery, IsRestart);

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
            CheckDeliveryFAMs(delivery, IsRestart);

        /// <summary>
        /// Determines whether [is valid adult skills (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is valid adult skills (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidAdultSkills(ILearningDelivery delivery) =>
            !IsRestart(delivery)
            && !IsLearnerInCustody(delivery)
            && !InApprenticeship(delivery)
            && HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills)
            && HasValidLearningAim(delivery, TypeOfLARSValidity.AdultSkills);

        /// <summary>
        /// Determines whether [is valid apprenticeship (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is valid apprenticeship (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidApprenticeship(ILearningDelivery delivery) =>
            !IsRestart(delivery)
            && !InStandardApprenticeship(delivery)
            && InApprenticeship(delivery)
            && IsComponentOfAProgram(delivery)
            && IsViableStart(delivery, ApprenticeshipMinimumStart, DateTime.Today)
            && HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills, TypeOfFunding.ApprenticeshipsFrom1May2017)
            && HasValidLearningAim(delivery, TypeOfLARSValidity.Apprenticeships);

        /// <summary>
        /// Determines whether [is valid unemployed (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid unemployed (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidUnemployed(ILearningDelivery delivery, ILearner learner) =>
            !IsRestart(delivery)
            && !IsLearnerInCustody(delivery)
            && !InApprenticeship(delivery)
            && IsViableStart(delivery, DateTime.MinValue, UnemployedMaximumStart)
            && InReceiptOfBenefitsAtStart(delivery, learner.LearnerEmploymentStatuses)
            && HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills)
            && HasValidLearningAim(delivery, TypeOfLARSValidity.Unemployed);

        /// <summary>
        /// Determines whether [is valid 16 to 19 efa (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is valid 16 to 19 efa (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValid16To19EFA(ILearningDelivery delivery) =>
            !IsRestart(delivery)
            && HasQualifyingFunding(delivery, TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfFunding.Other16To19)
            && HasValidLearningAim(delivery, TypeOfLARSValidity.EFA16To19);

        /// <summary>
        /// Determines whether [is valid community learning (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is valid community learning (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidCommunityLearning(ILearningDelivery delivery) =>
            !IsRestart(delivery)
            && HasQualifyingFunding(delivery, TypeOfFunding.CommunityLearning)
            && HasValidLearningAim(delivery, TypeOfLARSValidity.CommunityLearning);

        /// <summary>
        /// Determines whether [is valid olass (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is valid olass (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidOLASS(ILearningDelivery delivery) =>
            !IsRestart(delivery)
            && IsLearnerInCustody(delivery)
            && HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills)
            && HasValidLearningAim(delivery, TypeOfLARSValidity.OLASSAdult);

        /// <summary>
        /// Determines whether [is valid advanced learner loan (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is valid advanced learner loan] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidAdvancedLearnerLoan(ILearningDelivery delivery) =>
            !IsRestart(delivery)
            && IsAdvancedLearnerLoan(delivery)
            && HasQualifyingFunding(delivery, TypeOfFunding.NotFundedByESFA)
            && HasValidLearningAim(delivery, TypeOfLARSValidity.AdvancedLearnerLoan);

        /// <summary>
        /// Determines whether [is valid any (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is valid any (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidAny(ILearningDelivery delivery) =>
            !IsRestart(delivery)
            && !IsAdvancedLearnerLoan(delivery)
            && HasValidLearningAim(delivery, TypeOfLARSValidity.Any)
            && (HasQualifyingFunding(delivery, TypeOfFunding.NotFundedByESFA, TypeOfFunding.OtherAdult)
                || (HasQualifyingFunding(delivery, TypeOfFunding.ApprenticeshipsFrom1May2017)
                    && InStandardApprenticeship(delivery)));

        /// <summary>
        /// Determines whether [is valid esf (category)] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is valid esf (category)] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidESF(ILearningDelivery delivery) =>
            !IsRestart(delivery)
            && HasQualifyingFunding(delivery, TypeOfFunding.EuropeanSocialFund)
            && HasValidLearningAim(delivery, TypeOfLARSValidity.EuropeanSocialFund);

        /// <summary>
        /// Passes category restrictions.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>true if it does...</returns>
        public bool PassesCategoryRestrictions(ILearningDelivery delivery, ILearner learner) =>
            IsValidAdultSkills(delivery)
            || IsValidApprenticeship(delivery)
            || IsValidUnemployed(delivery, learner)
            || IsValid16To19EFA(delivery)
            || IsValidCommunityLearning(delivery)
            || IsValidOLASS(delivery)
            || IsValidAdvancedLearnerLoan(delivery)
            || IsValidAny(delivery)
            || IsValidESF(delivery);

        /// <summary>
        /// Passes the (rule) conditions.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>true if it does...</returns>
        public abstract bool PassesConditions(ILearningDelivery delivery, ILearner learner);

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery delivery, ILearner learner) =>
            PassesCategoryRestrictions(delivery, learner) && !PassesConditions(delivery, learner);

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
                .SafeWhere(x => IsNotValid(x, objectToValidate))
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, thisDelivery.LearnAimRef));

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}