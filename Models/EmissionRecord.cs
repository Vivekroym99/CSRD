using System;
using System.ComponentModel.DataAnnotations;

namespace CSRDReporting.Models
{
    /// <summary>
    /// Represents GHG emissions data for ESRS E1 Climate Change reporting
    /// Covers Scope 1, 2, and 3 emissions in accordance with GHG Protocol
    /// </summary>
    public class EmissionRecord
    {
        /// <summary>
        /// Unique identifier for the emission record
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Reporting period (fiscal year)
        /// </summary>
        [Required]
        public int ReportingYear { get; set; }

        /// <summary>
        /// Scope 1 emissions - Direct emissions from owned or controlled sources (tCO2e)
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Scope 1 emissions must be non-negative")]
        public decimal Scope1Emissions { get; set; }

        /// <summary>
        /// Scope 2 emissions - Indirect emissions from purchased energy (tCO2e)
        /// Location-based method
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Scope 2 emissions must be non-negative")]
        public decimal Scope2LocationBased { get; set; }

        /// <summary>
        /// Scope 2 emissions - Indirect emissions from purchased energy (tCO2e)
        /// Market-based method
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Scope 2 market-based emissions must be non-negative")]
        public decimal Scope2MarketBased { get; set; }

        /// <summary>
        /// Scope 3 emissions - All other indirect emissions (tCO2e)
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Scope 3 emissions must be non-negative")]
        public decimal Scope3Emissions { get; set; }

        /// <summary>
        /// Total emissions calculated field (tCO2e)
        /// </summary>
        public decimal TotalEmissions => Scope1Emissions + Scope2MarketBased + Scope3Emissions;

        /// <summary>
        /// Data quality assessment for emissions data
        /// </summary>
        public string DataQuality { get; set; } = string.Empty;

        /// <summary>
        /// Verification status (e.g., "Third-party verified", "Internal verification", "Unverified")
        /// </summary>
        public string VerificationStatus { get; set; } = "Unverified";

        /// <summary>
        /// Additional notes or methodology details
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Date when the record was created
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Date when the record was last modified
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// User who created/modified the record
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;
    }
}