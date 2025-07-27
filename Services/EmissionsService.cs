using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSRDReporting.Models;
using CSRDReporting.DataAccess;

namespace CSRDReporting.Services
{
    /// <summary>
    /// Service layer for emissions data business logic
    /// Handles validation, calculations, and business rules for emissions data
    /// </summary>
    public class EmissionsService
    {
        private readonly EmissionsRepository _repository;

        /// <summary>
        /// Initializes a new instance of the EmissionsService
        /// </summary>
        public EmissionsService()
        {
            _repository = new EmissionsRepository();
        }

        /// <summary>
        /// Gets all emission records
        /// </summary>
        public async Task<IEnumerable<EmissionRecord>> GetAllEmissionsAsync()
        {
            return await _repository.GetAllAsync();
        }

        /// <summary>
        /// Gets emission records for a specific year
        /// </summary>
        /// <param name="year">Reporting year</param>
        public async Task<IEnumerable<EmissionRecord>> GetEmissionsByYearAsync(int year)
        {
            return await _repository.GetByYearAsync(year);
        }

        /// <summary>
        /// Gets an emission record by ID
        /// </summary>
        /// <param name="id">Record ID</param>
        public async Task<EmissionRecord?> GetEmissionByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        /// <summary>
        /// Saves an emission record with business validation
        /// </summary>
        /// <param name="record">Emission record to save</param>
        /// <returns>Saved emission record</returns>
        public async Task<EmissionRecord> SaveEmissionAsync(EmissionRecord record)
        {
            ValidateEmissionRecord(record);

            if (record.Id == 0)
            {
                return await _repository.AddAsync(record);
            }
            else
            {
                return await _repository.UpdateAsync(record);
            }
        }

        /// <summary>
        /// Deletes an emission record
        /// </summary>
        /// <param name="id">Record ID to delete</param>
        public async Task<bool> DeleteEmissionAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        /// <summary>
        /// Validates emission record data according to ESRS E1 requirements
        /// </summary>
        /// <param name="record">Record to validate</param>
        private void ValidateEmissionRecord(EmissionRecord record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));

            if (record.ReportingYear < 2020 || record.ReportingYear > DateTime.Now.Year + 1)
                throw new ArgumentException("Reporting year must be between 2020 and next year.");

            if (record.Scope1Emissions < 0)
                throw new ArgumentException("Scope 1 emissions cannot be negative.");

            if (record.Scope2LocationBased < 0)
                throw new ArgumentException("Scope 2 location-based emissions cannot be negative.");

            if (record.Scope2MarketBased < 0)
                throw new ArgumentException("Scope 2 market-based emissions cannot be negative.");

            if (record.Scope3Emissions < 0)
                throw new ArgumentException("Scope 3 emissions cannot be negative.");

            // ESRS E1 validation: At least one scope should have data if record is being created
            if (record.TotalEmissions == 0)
            {
                // Warning: This might be acceptable for companies with no emissions
                // Consider if this should be a warning rather than an error
            }

            // Validate that market-based emissions are typically lower than location-based
            if (record.Scope2MarketBased > record.Scope2LocationBased && record.Scope2LocationBased > 0)
            {
                // This is unusual but not necessarily an error - could be due to RECs
                // Consider adding this as a warning in future versions
            }
        }

        /// <summary>
        /// Calculates emission trends for reporting
        /// </summary>
        /// <returns>Dictionary with years and total emissions</returns>
        public async Task<Dictionary<int, decimal>> GetEmissionTrendsAsync()
        {
            return await _repository.GetEmissionTrendsAsync();
        }

        /// <summary>
        /// Gets verification statistics for compliance reporting
        /// </summary>
        /// <returns>Dictionary with verification status and counts</returns>
        public async Task<Dictionary<string, int>> GetVerificationStatisticsAsync()
        {
            return await _repository.GetVerificationStatisticsAsync();
        }

        /// <summary>
        /// Calculates emission intensity per unit of revenue or production
        /// </summary>
        /// <param name="year">Reporting year</param>
        /// <param name="intensityBase">Base value for intensity calculation (revenue, production units, etc.)</param>
        /// <returns>Emission intensity ratio</returns>
        public async Task<decimal?> CalculateEmissionIntensityAsync(int year, decimal intensityBase)
        {
            if (intensityBase <= 0)
                return null;

            var emissions = await _repository.GetByYearAsync(year);
            decimal totalEmissions = 0;

            foreach (var emission in emissions)
            {
                totalEmissions += emission.TotalEmissions;
            }

            return totalEmissions / intensityBase;
        }

        /// <summary>
        /// Checks data completeness for ESRS E1 reporting requirements
        /// </summary>
        /// <param name="year">Reporting year</param>
        /// <returns>Data completeness assessment</returns>
        public async Task<EmissionDataCompleteness> AssessDataCompletenessAsync(int year)
        {
            var emissions = await _repository.GetByYearAsync(year);
            var assessment = new EmissionDataCompleteness { ReportingYear = year };

            foreach (var emission in emissions)
            {
                assessment.HasScope1Data = emission.Scope1Emissions > 0;
                assessment.HasScope2Data = emission.Scope2LocationBased > 0 || emission.Scope2MarketBased > 0;
                assessment.HasScope3Data = emission.Scope3Emissions > 0;
                assessment.HasVerification = !string.IsNullOrEmpty(emission.VerificationStatus) && 
                                           emission.VerificationStatus != "Unverified";
                assessment.HasDataQuality = !string.IsNullOrEmpty(emission.DataQuality);
            }

            assessment.CalculateCompleteness();
            return assessment;
        }

        /// <summary>
        /// Validates if emission reductions targets are being met
        /// </summary>
        /// <param name="baselineYear">Baseline year for comparison</param>
        /// <param name="currentYear">Current reporting year</param>
        /// <param name="targetReductionPercentage">Target reduction percentage</param>
        /// <returns>True if targets are being met</returns>
        public async Task<bool> ValidateEmissionTargetsAsync(int baselineYear, int currentYear, decimal targetReductionPercentage)
        {
            var baselineEmissions = await _repository.GetByYearAsync(baselineYear);
            var currentEmissions = await _repository.GetByYearAsync(currentYear);

            decimal baselineTotal = 0;
            decimal currentTotal = 0;

            foreach (var emission in baselineEmissions)
                baselineTotal += emission.TotalEmissions;

            foreach (var emission in currentEmissions)
                currentTotal += emission.TotalEmissions;

            if (baselineTotal == 0)
                return false;

            decimal actualReduction = (baselineTotal - currentTotal) / baselineTotal * 100;
            return actualReduction >= targetReductionPercentage;
        }
    }

    /// <summary>
    /// Data completeness assessment for ESRS E1 reporting
    /// </summary>
    public class EmissionDataCompleteness
    {
        public int ReportingYear { get; set; }
        public bool HasScope1Data { get; set; }
        public bool HasScope2Data { get; set; }
        public bool HasScope3Data { get; set; }
        public bool HasVerification { get; set; }
        public bool HasDataQuality { get; set; }
        public decimal CompletenessPercentage { get; private set; }

        public void CalculateCompleteness()
        {
            int totalChecks = 5;
            int passedChecks = 0;

            if (HasScope1Data) passedChecks++;
            if (HasScope2Data) passedChecks++;
            if (HasScope3Data) passedChecks++;
            if (HasVerification) passedChecks++;
            if (HasDataQuality) passedChecks++;

            CompletenessPercentage = (decimal)passedChecks / totalChecks * 100;
        }

        public string GetCompletenessStatus()
        {
            return CompletenessPercentage switch
            {
                >= 90 => "Excellent",
                >= 75 => "Good",
                >= 50 => "Adequate",
                >= 25 => "Poor",
                _ => "Incomplete"
            };
        }
    }
}