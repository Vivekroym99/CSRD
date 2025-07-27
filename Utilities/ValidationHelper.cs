using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CSRDReporting.Utilities
{
    /// <summary>
    /// Utility class for data validation across the CSRD application
    /// Provides centralized validation logic for ESRS compliance
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Validates an object using data annotations
        /// </summary>
        /// <param name="obj">Object to validate</param>
        /// <returns>List of validation results</returns>
        public static List<ValidationResult> ValidateObject(object obj)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(obj);
            
            Validator.TryValidateObject(obj, validationContext, validationResults, true);
            
            return validationResults;
        }

        /// <summary>
        /// Validates an object and throws an exception if validation fails
        /// </summary>
        /// <param name="obj">Object to validate</param>
        /// <param name="objectName">Name of the object for error messages</param>
        public static void ValidateObjectOrThrow(object obj, string objectName = "Object")
        {
            var validationResults = ValidateObject(obj);
            
            if (validationResults.Any())
            {
                var errorMessages = validationResults.Select(vr => vr.ErrorMessage);
                var combinedMessage = string.Join("\n", errorMessages);
                throw new ValidationException($"{objectName} validation failed:\n{combinedMessage}");
            }
        }

        /// <summary>
        /// Validates reporting year for CSRD compliance
        /// </summary>
        /// <param name="year">Year to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsValidReportingYear(int year)
        {
            const int firstCSRDYear = 2024; // CSRD applies from 2024
            int currentYear = DateTime.Now.Year;
            int maxFutureYear = currentYear + 1;

            return year >= firstCSRDYear && year <= maxFutureYear;
        }

        /// <summary>
        /// Validates GHG emissions values according to GHG Protocol standards
        /// </summary>
        /// <param name="scope1">Scope 1 emissions</param>
        /// <param name="scope2Location">Scope 2 location-based emissions</param>
        /// <param name="scope2Market">Scope 2 market-based emissions</param>
        /// <param name="scope3">Scope 3 emissions</param>
        /// <returns>List of validation errors</returns>
        public static List<string> ValidateGHGEmissions(decimal scope1, decimal scope2Location, decimal scope2Market, decimal scope3)
        {
            var errors = new List<string>();

            if (scope1 < 0)
                errors.Add("Scope 1 emissions cannot be negative.");

            if (scope2Location < 0)
                errors.Add("Scope 2 location-based emissions cannot be negative.");

            if (scope2Market < 0)
                errors.Add("Scope 2 market-based emissions cannot be negative.");

            if (scope3 < 0)
                errors.Add("Scope 3 emissions cannot be negative.");

            // Business rule: Market-based is typically lower than location-based due to renewable energy purchases
            if (scope2Market > scope2Location && scope2Location > 0)
            {
                errors.Add("Warning: Market-based Scope 2 emissions are higher than location-based. Please verify data.");
            }

            // Business rule: At least one emission scope should have data
            if (scope1 == 0 && scope2Location == 0 && scope2Market == 0 && scope3 == 0)
            {
                errors.Add("Warning: All emission scopes are zero. This may indicate missing data.");
            }

            return errors;
        }

        /// <summary>
        /// Validates energy consumption data for ESRS E1 compliance
        /// </summary>
        /// <param name="consumptionMWh">Energy consumption in MWh</param>
        /// <param name="isRenewable">Whether energy source is renewable</param>
        /// <param name="energySource">Type of energy source</param>
        /// <returns>List of validation errors</returns>
        public static List<string> ValidateEnergyConsumption(decimal consumptionMWh, bool isRenewable, string energySource)
        {
            var errors = new List<string>();

            if (consumptionMWh < 0)
                errors.Add("Energy consumption cannot be negative.");

            if (string.IsNullOrWhiteSpace(energySource))
                errors.Add("Energy source must be specified.");

            // Business rules for renewable energy classification
            var renewableSources = new[] { "solar", "wind", "hydroelectric", "geothermal", "biomass" };
            var nonRenewableSources = new[] { "coal", "natural gas", "oil", "nuclear" };

            if (isRenewable && nonRenewableSources.Any(source => energySource.ToLower().Contains(source)))
            {
                errors.Add($"Warning: '{energySource}' is marked as renewable but appears to be a non-renewable source.");
            }

            if (!isRenewable && renewableSources.Any(source => energySource.ToLower().Contains(source)))
            {
                errors.Add($"Warning: '{energySource}' is marked as non-renewable but appears to be a renewable source.");
            }

            return errors;
        }

        /// <summary>
        /// Validates workforce diversity data for ESRS S1 compliance
        /// </summary>
        /// <param name="totalEmployees">Total number of employees</param>
        /// <param name="femaleEmployees">Number of female employees</param>
        /// <param name="maleEmployees">Number of male employees</param>
        /// <param name="nonBinaryEmployees">Number of non-binary employees</param>
        /// <returns>List of validation errors</returns>
        public static List<string> ValidateWorkforceDiversity(int totalEmployees, int femaleEmployees, int maleEmployees, int nonBinaryEmployees)
        {
            var errors = new List<string>();

            if (totalEmployees < 0)
                errors.Add("Total employees cannot be negative.");

            if (femaleEmployees < 0)
                errors.Add("Female employees cannot be negative.");

            if (maleEmployees < 0)
                errors.Add("Male employees cannot be negative.");

            if (nonBinaryEmployees < 0)
                errors.Add("Non-binary employees cannot be negative.");

            // Business rule: Gender breakdown should match total
            int genderTotal = femaleEmployees + maleEmployees + nonBinaryEmployees;
            if (genderTotal != totalEmployees && totalEmployees > 0)
            {
                errors.Add($"Gender breakdown ({genderTotal}) does not match total employees ({totalEmployees}).");
            }

            return errors;
        }

        /// <summary>
        /// Validates materiality assessment scoring
        /// </summary>
        /// <param name="impactMateriality">Impact materiality score (1-5)</param>
        /// <param name="financialMateriality">Financial materiality score (1-5)</param>
        /// <returns>List of validation errors</returns>
        public static List<string> ValidateMaterialityScoring(int impactMateriality, int financialMateriality)
        {
            var errors = new List<string>();

            if (impactMateriality < 1 || impactMateriality > 5)
                errors.Add("Impact materiality score must be between 1 and 5.");

            if (financialMateriality < 1 || financialMateriality > 5)
                errors.Add("Financial materiality score must be between 1 and 5.");

            return errors;
        }

        /// <summary>
        /// Validates data quality indicators for ESRS compliance
        /// </summary>
        /// <param name="dataQuality">Data quality level</param>
        /// <param name="verificationStatus">Verification status</param>
        /// <returns>List of validation warnings</returns>
        public static List<string> ValidateDataQuality(string dataQuality, string verificationStatus)
        {
            var warnings = new List<string>();

            var validQualityLevels = new[] { "High", "Medium", "Low", "Estimated" };
            var validVerificationStatuses = new[] { "Unverified", "Internal verification", "Third-party verified" };

            if (!string.IsNullOrEmpty(dataQuality) && !validQualityLevels.Contains(dataQuality))
                warnings.Add($"'{dataQuality}' is not a standard data quality level.");

            if (!string.IsNullOrEmpty(verificationStatus) && !validVerificationStatuses.Contains(verificationStatus))
                warnings.Add($"'{verificationStatus}' is not a standard verification status.");

            // Business rule: High data quality should have verification
            if (dataQuality == "High" && verificationStatus == "Unverified")
                warnings.Add("High data quality typically requires some form of verification.");

            return warnings;
        }

        /// <summary>
        /// Validates Polish CSRD specific requirements (placeholder for future implementation)
        /// </summary>
        /// <param name="reportingYear">Reporting year</param>
        /// <returns>List of validation messages specific to Polish implementation</returns>
        public static List<string> ValidatePolishCSRDRequirements(int reportingYear)
        {
            var messages = new List<string>();

            // Placeholder for Polish-specific validations
            // These would be implemented based on Polish transposition of CSRD
            
            if (reportingYear >= 2024)
            {
                messages.Add("Info: Ensure compliance with Polish CSRD transposition requirements.");
                messages.Add("Info: Consider Polish National Reporting Authority guidelines.");
            }

            return messages;
        }

        /// <summary>
        /// Comprehensive validation for ESRS data completeness
        /// </summary>
        /// <param name="reportingYear">Year being reported</param>
        /// <param name="hasEnvironmentalData">Whether environmental data exists</param>
        /// <param name="hasSocialData">Whether social data exists</param>
        /// <param name="hasGovernanceData">Whether governance data exists</param>
        /// <param name="hasMaterialityAssessment">Whether materiality assessment exists</param>
        /// <returns>Data completeness assessment</returns>
        public static ESRSDataCompleteness ValidateESRSCompleteness(int reportingYear, bool hasEnvironmentalData, 
            bool hasSocialData, bool hasGovernanceData, bool hasMaterialityAssessment)
        {
            var assessment = new ESRSDataCompleteness
            {
                ReportingYear = reportingYear,
                HasEnvironmentalData = hasEnvironmentalData,
                HasSocialData = hasSocialData,
                HasGovernanceData = hasGovernanceData,
                HasMaterialityAssessment = hasMaterialityAssessment
            };

            assessment.CalculateCompleteness();
            return assessment;
        }
    }

    /// <summary>
    /// ESRS data completeness assessment
    /// </summary>
    public class ESRSDataCompleteness
    {
        public int ReportingYear { get; set; }
        public bool HasEnvironmentalData { get; set; }
        public bool HasSocialData { get; set; }
        public bool HasGovernanceData { get; set; }
        public bool HasMaterialityAssessment { get; set; }
        public decimal CompletenessPercentage { get; private set; }
        public List<string> MissingDataPoints { get; private set; } = new List<string>();

        public void CalculateCompleteness()
        {
            int totalCategories = 4;
            int completedCategories = 0;

            if (HasEnvironmentalData) completedCategories++;
            else MissingDataPoints.Add("Environmental data (ESRS E)");

            if (HasSocialData) completedCategories++;
            else MissingDataPoints.Add("Social data (ESRS S)");

            if (HasGovernanceData) completedCategories++;
            else MissingDataPoints.Add("Governance data (ESRS G)");

            if (HasMaterialityAssessment) completedCategories++;
            else MissingDataPoints.Add("Double materiality assessment");

            CompletenessPercentage = (decimal)completedCategories / totalCategories * 100;
        }

        public string GetCompletenessStatus()
        {
            return CompletenessPercentage switch
            {
                100 => "Complete",
                >= 75 => "Mostly Complete",
                >= 50 => "Partially Complete",
                >= 25 => "Limited Data",
                _ => "Insufficient Data"
            };
        }
    }
}