using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpId
{
    public class EmpId_13RuleTests : AbstractRuleTests<EmpId_13Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("EmpId_13");
        }

        private EmpId_13Rule NewRule(
            IDD07 dd07 = null,
            IFileDataService fileDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new EmpId_13Rule(dd07, fileDataService, validationErrorHandler);
        }
    }
}
