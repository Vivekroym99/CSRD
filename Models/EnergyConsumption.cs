using System;
using System.ComponentModel.DataAnnotations;

namespace CSRDReporting.Models
{
    /// <summary>
    /// Represents energy consumption data for ESRS E1 Climate Change reporting
    /// Tracks renewable and non-renewable energy consumption
    /// </summary>
    public class EnergyConsumption
    {
        /// <summary>
        /// Unique identifier for the energy consumption record
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Reporting period (fiscal year)
        /// </summary>
        [Required]
        public int ReportingYear { get; set; }

        /// <summary>
        /// Energy source type (e.g., "Electricity", "Natural Gas", "Solar", "Wind")
        /// </summary>
        [Required]
        public string EnergySource { get; set; } = string.Empty;

        /// <summary>
        /// Energy consumption in MWh
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Energy consumption must be non-negative")]
        public decimal ConsumptionMWh { get; set; }

        /// <summary>
        /// Indicates if the energy source is renewable
        /// </summary>
        public bool IsRenewable { get; set; }

        /// <summary>
        /// Grid electricity consumption (MWh)
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal GridElectricity { get; set; }

        /// <summary>
        /// Self-generated renewable energy (MWh)
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal SelfGeneratedRenewable { get; set; }

        /// <summary>
        /// Purchased renewable energy certificates (MWh)
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal PurchasedRenewableCertificates { get; set; }

        /// <summary>
        /// Energy intensity ratio (MWh per unit of production/revenue)
        /// </summary>
        public decimal? EnergyIntensity { get; set; }

        /// <summary>
        /// Data collection methodology
        /// </summary>
        public string Methodology { get; set; } = string.Empty;

        /// <summary>
        /// Additional notes
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