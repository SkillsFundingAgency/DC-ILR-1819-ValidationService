using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.InternalData.ValidationDataService
{
    public class ValidationDataService : IValidationDataService
    {
        private readonly DateTime _academicYearAugustThirtyFirst = new DateTime(2017, 8, 31);
        private readonly DateTime _academicYearEnd = new DateTime(2018, 7, 31);
        private readonly DateTime _academicYearJanuaryFirst = new DateTime(2018, 1, 1);
        private readonly DateTime _academicYearStart = new DateTime(2017, 8, 1);
        private readonly IReadOnlyCollection<long> _apprenticeshipProgTypes = new HashSet<long>() { 2, 3, 20, 21, 2, 23, 25 };
        private readonly DateTime _apprenticeshipProgAllowedStartDate = new DateTime(2016, 08, 01);
        private readonly DateTime _validationStartDateTime;

        public ValidationDataService(IDateTimeProvider dateTimeProvider)
        {
            _validationStartDateTime = dateTimeProvider?.UtcNow ?? DateTime.UtcNow;
        }

        public DateTime AcademicYearAugustThirtyFirst
        {
            get { return _academicYearAugustThirtyFirst;  }
        }

        public DateTime AcademicYearEnd
        {
            get { return _academicYearEnd; }
        }

        public DateTime AcademicYearJanuaryFirst
        {
            get { return _academicYearJanuaryFirst; }
        }

        public DateTime AcademicYearStart
        {
            get { return _academicYearStart; }
        }

        public IReadOnlyCollection<long> ApprenticeProgTypes
        {
            get { return _apprenticeshipProgTypes; }
        }

        public DateTime ApprencticeProgAllowedStartDate
        {
            get { return _apprenticeshipProgAllowedStartDate; }
        }

        public DateTime ValidationStartDateTime
        {
            get { return _validationStartDateTime; }
        }
    }
}
