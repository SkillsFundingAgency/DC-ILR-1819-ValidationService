using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    public static class CompStatus
    {
        public const int LearnerContinuingOrIntendingToContinue = 1;

        public const int LearnerCompletedTheLearningActivities = 2;

        public const int LearnerWithdrawFromLearningActivities = 3;

        public const int LearnerTemporarilyWithdrawn = 6;
    }
}
