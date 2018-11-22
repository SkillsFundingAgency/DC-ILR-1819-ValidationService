﻿namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    /// <summary>
    /// derived data rule 05
    /// Checksum digit for Employer reference number
    /// </summary>
    public interface IDerivedData_05Rule
    {
        /// <summary>
        /// Gets the employer identifier checksum.
        /// </summary>
        /// <param name="thisEmployer">this employer.</param>
        /// <returns>the checksum character</returns>
        char GetEmployerIDChecksum(int thisEmployer);
    }
}
