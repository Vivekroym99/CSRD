using System;
using System.ComponentModel.DataAnnotations;

namespace CSRDReporting.Models
{
    /// <summary>
    /// Represents workforce diversity data for ESRS S1 Own Workforce reporting
    /// Tracks gender, age, and other diversity metrics
    /// </summary>
    public class WorkforceDiversity
    {
        /// <summary>
        /// Unique identifier for the workforce diversity record
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Reporting period (fiscal year)
        /// </summary>
        [Required]
        public int ReportingYear { get; set; }

        /// <summary>
        /// Employee category (e.g., "Management", "Technical", "Administrative")
        /// </summary>
        [Required]
        public string EmployeeCategory { get; set; } = string.Empty;

        /// <summary>
        /// Total number of employees in this category
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Total employees must be non-negative")]
        public int TotalEmployees { get; set; }

        /// <summary>
        /// Number of female employees
        /// </summary>
        [Range(0, int.MaxValue)]
        public int FemaleEmployees { get; set; }

        /// <summary>
        /// Number of male employees
        /// </summary>
        [Range(0, int.MaxValue)]
        public int MaleEmployees { get; set; }

        /// <summary>
        /// Number of employees with non-binary gender identity
        /// </summary>
        [Range(0, int.MaxValue)]
        public int NonBinaryEmployees { get; set; }

        /// <summary>
        /// Number of employees under 30 years old
        /// </summary>
        [Range(0, int.MaxValue)]
        public int EmployeesUnder30 { get; set; }

        /// <summary>
        /// Number of employees between 30-50 years old
        /// </summary>
        [Range(0, int.MaxValue)]
        public int Employees30To50 { get; set; }

        /// <summary>
        /// Number of employees over 50 years old
        /// </summary>
        [Range(0, int.MaxValue)]
        public int EmployeesOver50 { get; set; }

        /// <summary>
        /// Number of employees with disabilities
        /// </summary>
        [Range(0, int.MaxValue)]
        public int EmployeesWithDisabilities { get; set; }

        /// <summary>
        /// Number of employees from ethnic minorities
        /// </summary>
        [Range(0, int.MaxValue)]
        public int EthnicMinorityEmployees { get; set; }

        /// <summary>
        /// Average training hours per employee
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal AverageTrainingHours { get; set; }

        /// <summary>
        /// Employee turnover rate (percentage)
        /// </summary>
        [Range(0, 100)]
        public decimal TurnoverRate { get; set; }

        /// <summary>
        /// Gender pay gap (percentage)
        /// </summary>
        public decimal? GenderPayGap { get; set; }

        /// <summary>
        /// Additional notes on diversity initiatives
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

        /// <summary>
        /// Calculated percentage of female employees
        /// </summary>
        public decimal FemalePercentage => TotalEmployees > 0 ? (decimal)FemaleEmployees / TotalEmployees * 100 : 0;

        /// <summary>
        /// Calculated percentage of employees with disabilities
        /// </summary>
        public decimal DisabilityPercentage => TotalEmployees > 0 ? (decimal)EmployeesWithDisabilities / TotalEmployees * 100 : 0;
    }
}