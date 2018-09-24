namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    /// <summary>
    /// type(s) of LARS basic skill
    /// </summary>
    public class TypeOfLARSBasicSkill
    {
        /// <summary>
        /// not applicable
        /// </summary>
        public const int NotApplicable = -2;

        /// <summary>
        /// unknown
        /// </summary>
        public const int Unknown = -1;

        /// <summary>
        /// certificate (in) adult literacy
        /// </summary>
        public const int Certificate_AdultLiteracy = 1;

        /// <summary>
        /// certificate (in) adult numeracy
        /// </summary>
        public const int Certificate_AdultNumeracy = 2;

        /// <summary>
        /// gcse (in) english language
        /// </summary>
        public const int GCSE_EnglishLanguage = 11;

        /// <summary>
        /// gcse (in) mathematics
        /// </summary>
        public const int GCSE_Mathematics = 12;

        /// <summary>
        /// key skill (in) communication
        /// </summary>
        public const int KeySkill_Communication = 13;

        /// <summary>
        /// key skill (in) application of numbers
        /// </summary>
        public const int KeySkill_ApplicationOfNumbers = 14;

        /// <summary>
        /// other S4L (skills for learning) not literacy numeracy or ESOL
        /// </summary>
        public const int OtherS4LNotLiteracyNumeracyOrESOL = 18;

        /// <summary>
        /// functional skills (in) mathematics
        /// </summary>
        public const int FunctionalSkillsMathematics = 19;

        /// <summary>
        /// functional skills (in) english
        /// </summary>
        public const int FunctionalSkillsEnglish = 20;

        /// <summary>
        /// units of the certificate (in) adult numeracy
        /// </summary>
        public const int UnitsOfTheCertificate_AdultNumeracy = 21;

        /// <summary>
        /// units of the certificate ESOL S4L (skills for learning)
        /// </summary>
        public const int UnitsOfTheCertificate_ESOLS4L = 22;

        /// <summary>
        /// units of the certificate (in) adult literacy
        /// </summary>
        public const int UnitsOfTheCertificate_AdultLiteracy = 23;

        /// <summary>
        /// non NQF QCF S4L (skills for learning) literacy
        /// </summary>
        public const int NonNQF_QCFS4LLiteracy = 24;

        /// <summary>
        /// non NQF QCF S4L (skills for learning) numeracy
        /// </summary>
        public const int NonNQF_QCFS4LNumeracy = 25;

        /// <summary>
        /// non NQF QCF S4L (skills for learning) ESOL
        /// </summary>
        public const int NonNQF_QCFS4LESOL = 26;

        /// <summary>
        /// certificate ESOL S4L (skills for learning)
        /// </summary>
        public const int CertificateESOLS4L = 27;

        /// <summary>
        /// certificate GCSE S4L (skills for learning) speaking and listening
        /// </summary>
        public const int CertificateESOLS4LSpeakListen = 28;

        /// <summary>
        /// QCF basic skills (in) english language
        /// </summary>
        public const int QCFBasicSkillsEnglishLanguage = 29;

        /// <summary>
        /// QCF basic skills (in) mathematics
        /// </summary>
        public const int QCFBasicSkillsMathematics = 30;

        /// <summary>
        /// unit QCF basic skills (in) english language
        /// </summary>
        public const int UnitQCFBasicSkillsEnglishLanguage = 31;

        /// <summary>
        /// unit QCF basic skills (in) mathematics
        /// </summary>
        public const int UnitQCFBasicSkillsMathematics = 32;

        /// <summary>
        /// international GCSE (in) english language
        /// </summary>
        public const int InternationalGCSEEnglishLanguage = 33;

        /// <summary>
        /// international GCSE (in) mathematics
        /// </summary>
        public const int InternationalGCSEMathematics = 34;

        /// <summary>
        /// free standing mathematics qualification
        /// </summary>
        public const int FreeStandingMathematicsQualification = 35;

        /// <summary>
        /// QCF certificate ESOL
        /// </summary>
        public const int QCFCertificateESOL = 36;

        /// <summary>
        /// QCF ESOL speaking and listening
        /// </summary>
        public const int QCFESOLSpeakListen = 37;

        /// <summary>
        /// QCF ESOL reading
        /// </summary>
        public const int QCFESOLReading = 38;

        /// <summary>
        /// QCF ESOL writing
        /// </summary>
        public const int QCFESOLWriting = 39;

        /// <summary>
        /// unit ESOL speaking and listening
        /// </summary>
        public const int UnitESOLSpeakListen = 40;

        /// <summary>
        /// unit ESOL reading
        /// </summary>
        public const int UnitESOLReading = 41;

        /// <summary>
        /// unit ESOL writing
        /// </summary>
        public const int UnitESOLWriting = 42;

        public static readonly int[] AsEnglishAndMathsBasicSkills = new int[]
        {
            Certificate_AdultLiteracy,
            Certificate_AdultNumeracy,
            GCSE_EnglishLanguage,
            GCSE_Mathematics,
            KeySkill_Communication,
            KeySkill_ApplicationOfNumbers,
            FunctionalSkillsMathematics,
            FunctionalSkillsEnglish,
            UnitsOfTheCertificate_AdultNumeracy,
            UnitsOfTheCertificate_AdultLiteracy,
            NonNQF_QCFS4LLiteracy,
            NonNQF_QCFS4LNumeracy,
            QCFBasicSkillsEnglishLanguage,
            QCFBasicSkillsMathematics,
            UnitQCFBasicSkillsEnglishLanguage,
            UnitQCFBasicSkillsMathematics,
            InternationalGCSEEnglishLanguage,
            InternationalGCSEMathematics,
            FreeStandingMathematicsQualification
        };

        /// <summary>
        /// As ESOL (English for Speakers of Other Languages) basic skills
        /// </summary>
        public static readonly int[] AsESOLBasicSkills = new int[]
        {
            QCFCertificateESOL,
            QCFESOLSpeakListen,
            QCFESOLReading,
            QCFESOLWriting,
            UnitESOLSpeakListen,
            UnitESOLReading,
            UnitESOLWriting
        };
    }
}
