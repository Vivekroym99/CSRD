using System;
using System.ComponentModel.DataAnnotations;

namespace CSRDReporting.Models
{
    /// <summary>
    /// Represents a materiality assessment record for double materiality analysis
    /// Covers both impact materiality and financial materiality as required by ESRS
    /// </summary>
    public class MaterialityAssessment
    {
        /// <summary>
        /// Unique identifier for the materiality assessment
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Reporting period (fiscal year)
        /// </summary>
        [Required]
        public int ReportingYear { get; set; }

        /// <summary>
        /// Sustainability topic (e.g., "Climate Change", "Biodiversity", "Human Rights")
        /// </summary>
        [Required]
        public string SustainabilityTopic { get; set; } = string.Empty;

        /// <summary>
        /// Related ESRS standard (e.g., "ESRS E1", "ESRS S1")
        /// </summary>
        public string ESRSStandard { get; set; } = string.Empty;

        /// <summary>
        /// Impact materiality assessment - Impact of company on environment/society
        /// Scale: 1-5 (1=Low impact, 5=High impact)
        /// </summary>
        [Range(1, 5, ErrorMessage = "Impact materiality must be between 1 and 5")]
        public int ImpactMateriality { get; set; }

        /// <summary>
        /// Financial materiality assessment - Impact of sustainability topic on company value
        /// Scale: 1-5 (1=Low impact, 5=High impact)
        /// </summary>
        [Range(1, 5, ErrorMessage = "Financial materiality must be between 1 and 5")]
        public int FinancialMateriality { get; set; }

        /// <summary>
        /// Overall materiality determination based on double materiality
        /// </summary>
        public bool IsMaterial => ImpactMateriality >= 3 || FinancialMateriality >= 3;

        /// <summary>
        /// Time horizon for the assessment (Short-term, Medium-term, Long-term)
        /// </summary>
        public string TimeHorizon { get; set; } = string.Empty;

        /// <summary>
        /// Stakeholder groups consulted in the assessment
        /// </summary>
        public string StakeholderGroups { get; set; } = string.Empty;

        /// <summary>
        /// Methodology used for the assessment
        /// </summary>
        public string AssessmentMethodology { get; set; } = string.Empty;

        /// <summary>
        /// Key risks identified related to this topic
        /// </summary>
        public string IdentifiedRisks { get; set; } = string.Empty;

        /// <summary>
        /// Key opportunities identified related to this topic
        /// </summary>
        public string IdentifiedOpportunities { get; set; } = string.Empty;

        /// <summary>
        /// Supporting evidence and data sources
        /// </summary>
        public string SupportingEvidence { get; set; } = string.Empty;

        /// <summary>
        /// Management response and planned actions
        /// </summary>
        public string ManagementResponse { get; set; } = string.Empty;

        /// <summary>
        /// Review status of the assessment
        /// </summary>
        public string ReviewStatus { get; set; } = "Draft";

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
        /// Calculated materiality score (highest of impact or financial)
        /// </summary>
        public int MaterialityScore => Math.Max(ImpactMateriality, FinancialMateriality);
    }
}