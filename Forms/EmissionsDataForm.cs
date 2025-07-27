using System;
using System.Drawing;
using System.Windows.Forms;
using CSRDReporting.Models;
using CSRDReporting.DataAccess;

namespace CSRDReporting.Forms
{
    /// <summary>
    /// Form for collecting ESRS E1 Climate Change emissions data
    /// Supports Scope 1, 2, and 3 GHG emissions reporting
    /// </summary>
    public partial class EmissionsDataForm : Form
    {
        private EmissionRecord currentRecord;
        private bool isEditMode;

        // Form controls
        private GroupBox dataGroupBox;
        private GroupBox scope1GroupBox;
        private GroupBox scope2GroupBox;
        private GroupBox scope3GroupBox;
        private GroupBox metadataGroupBox;

        private NumericUpDown reportingYearControl;
        private NumericUpDown scope1EmissionsControl;
        private NumericUpDown scope2LocationBasedControl;
        private NumericUpDown scope2MarketBasedControl;
        private NumericUpDown scope3EmissionsControl;
        private ComboBox dataQualityControl;
        private ComboBox verificationStatusControl;
        private TextBox notesControl;

        private Label totalEmissionsLabel;
        private Button saveButton;
        private Button cancelButton;
        private Button clearButton;

        /// <summary>
        /// Initializes a new instance of the EmissionsDataForm
        /// </summary>
        public EmissionsDataForm()
        {
            InitializeComponent();
            SetupForm();
            SetupControls();
            LoadNewRecord();
        }

        /// <summary>
        /// Initializes the form for editing an existing record
        /// </summary>
        /// <param name="record">The emission record to edit</param>
        public EmissionsDataForm(EmissionRecord record) : this()
        {
            currentRecord = record;
            isEditMode = true;
            LoadRecord();
        }

        /// <summary>
        /// Sets up the basic form properties
        /// </summary>
        private void SetupForm()
        {
            Text = "ESRS E1: Climate Change - GHG Emissions Data";
            Size = new Size(800, 700);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(700, 600);
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        /// <summary>
        /// Sets up all form controls and layout
        /// </summary>
        private void SetupControls()
        {
            var titleLabel = new Label
            {
                Text = "GHG Emissions Data Entry (ESRS E1)",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(400, 30)
            };

            var descriptionLabel = new Label
            {
                Text = "Enter greenhouse gas emissions data according to GHG Protocol standards.",
                Location = new Point(20, 55),
                Size = new Size(600, 20),
                ForeColor = Color.DarkBlue
            };

            SetupDataGroupBox();
            SetupScope1GroupBox();
            SetupScope2GroupBox();
            SetupScope3GroupBox();
            SetupMetadataGroupBox();
            SetupActionButtons();

            Controls.AddRange(new Control[] 
            { 
                titleLabel, 
                descriptionLabel,
                dataGroupBox,
                scope1GroupBox, 
                scope2GroupBox, 
                scope3GroupBox,
                metadataGroupBox,
                saveButton, 
                cancelButton, 
                clearButton 
            });
        }

        /// <summary>
        /// Sets up the basic data group box with reporting year
        /// </summary>
        private void SetupDataGroupBox()
        {
            dataGroupBox = new GroupBox
            {
                Text = "Reporting Period",
                Location = new Point(20, 90),
                Size = new Size(740, 70),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            var yearLabel = new Label
            {
                Text = "Reporting Year:",
                Location = new Point(20, 30),
                Size = new Size(100, 20)
            };

            reportingYearControl = new NumericUpDown
            {
                Location = new Point(130, 28),
                Size = new Size(80, 23),
                Minimum = 2020,
                Maximum = 2050,
                Value = DateTime.Now.Year
            };

            dataGroupBox.Controls.AddRange(new Control[] { yearLabel, reportingYearControl });
        }

        /// <summary>
        /// Sets up Scope 1 emissions input controls
        /// </summary>
        private void SetupScope1GroupBox()
        {
            scope1GroupBox = new GroupBox
            {
                Text = "Scope 1 Emissions (Direct emissions from owned/controlled sources)",
                Location = new Point(20, 170),
                Size = new Size(740, 80),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            var scope1Label = new Label
            {
                Text = "Scope 1 Emissions (tCO2e):",
                Location = new Point(20, 30),
                Size = new Size(150, 20)
            };

            scope1EmissionsControl = new NumericUpDown
            {
                Location = new Point(180, 28),
                Size = new Size(120, 23),
                DecimalPlaces = 2,
                Maximum = 999999999,
                ThousandsSeparator = true
            };

            var scope1InfoLabel = new Label
            {
                Text = "Include: Company vehicles, heating, manufacturing processes, fugitive emissions",
                Location = new Point(320, 30),
                Size = new Size(400, 20),
                ForeColor = Color.DarkGreen,
                Font = new Font("Segoe UI", 8F, FontStyle.Italic)
            };

            scope1GroupBox.Controls.AddRange(new Control[] { scope1Label, scope1EmissionsControl, scope1InfoLabel });
        }

        /// <summary>
        /// Sets up Scope 2 emissions input controls
        /// </summary>
        private void SetupScope2GroupBox()
        {
            scope2GroupBox = new GroupBox
            {
                Text = "Scope 2 Emissions (Indirect emissions from purchased energy)",
                Location = new Point(20, 260),
                Size = new Size(740, 120),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            var scope2LocationLabel = new Label
            {
                Text = "Location-based (tCO2e):",
                Location = new Point(20, 30),
                Size = new Size(140, 20)
            };

            scope2LocationBasedControl = new NumericUpDown
            {
                Location = new Point(170, 28),
                Size = new Size(120, 23),
                DecimalPlaces = 2,
                Maximum = 999999999,
                ThousandsSeparator = true
            };

            var scope2MarketLabel = new Label
            {
                Text = "Market-based (tCO2e):",
                Location = new Point(20, 60),
                Size = new Size(140, 20)
            };

            scope2MarketBasedControl = new NumericUpDown
            {
                Location = new Point(170, 58),
                Size = new Size(120, 23),
                DecimalPlaces = 2,
                Maximum = 999999999,
                ThousandsSeparator = true
            };

            var scope2InfoLabel = new Label
            {
                Text = "Location-based: Uses average emission factors for electricity grid\n" +
                       "Market-based: Reflects contractual arrangements (RECs, PPAs)",
                Location = new Point(320, 30),
                Size = new Size(400, 40),
                ForeColor = Color.DarkGreen,
                Font = new Font("Segoe UI", 8F, FontStyle.Italic)
            };

            scope2GroupBox.Controls.AddRange(new Control[] 
            { 
                scope2LocationLabel, scope2LocationBasedControl,
                scope2MarketLabel, scope2MarketBasedControl,
                scope2InfoLabel 
            });
        }

        /// <summary>
        /// Sets up Scope 3 emissions input controls
        /// </summary>
        private void SetupScope3GroupBox()
        {
            scope3GroupBox = new GroupBox
            {
                Text = "Scope 3 Emissions (Other indirect emissions in value chain)",
                Location = new Point(20, 390),
                Size = new Size(740, 80),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            var scope3Label = new Label
            {
                Text = "Scope 3 Emissions (tCO2e):",
                Location = new Point(20, 30),
                Size = new Size(150, 20)
            };

            scope3EmissionsControl = new NumericUpDown
            {
                Location = new Point(180, 28),
                Size = new Size(120, 23),
                DecimalPlaces = 2,
                Maximum = 999999999,
                ThousandsSeparator = true
            };

            var scope3InfoLabel = new Label
            {
                Text = "Include: Business travel, employee commuting, purchased goods, waste, transportation",
                Location = new Point(320, 30),
                Size = new Size(400, 20),
                ForeColor = Color.DarkGreen,
                Font = new Font("Segoe UI", 8F, FontStyle.Italic)
            };

            scope3GroupBox.Controls.AddRange(new Control[] { scope3Label, scope3EmissionsControl, scope3InfoLabel });
        }

        /// <summary>
        /// Sets up metadata and quality controls
        /// </summary>
        private void SetupMetadataGroupBox()
        {
            metadataGroupBox = new GroupBox
            {
                Text = "Data Quality & Verification",
                Location = new Point(20, 480),
                Size = new Size(740, 120),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            var dataQualityLabel = new Label
            {
                Text = "Data Quality:",
                Location = new Point(20, 30),
                Size = new Size(80, 20)
            };

            dataQualityControl = new ComboBox
            {
                Location = new Point(110, 28),
                Size = new Size(150, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            dataQualityControl.Items.AddRange(new[] { "High", "Medium", "Low", "Estimated" });

            var verificationLabel = new Label
            {
                Text = "Verification:",
                Location = new Point(280, 30),
                Size = new Size(80, 20)
            };

            verificationStatusControl = new ComboBox
            {
                Location = new Point(370, 28),
                Size = new Size(150, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            verificationStatusControl.Items.AddRange(new[] { "Unverified", "Internal verification", "Third-party verified" });

            var notesLabel = new Label
            {
                Text = "Notes:",
                Location = new Point(20, 60),
                Size = new Size(50, 20)
            };

            notesControl = new TextBox
            {
                Location = new Point(80, 58),
                Size = new Size(640, 23),
                PlaceholderText = "Additional notes, methodology details, data sources..."
            };

            totalEmissionsLabel = new Label
            {
                Text = "Total Emissions: 0.00 tCO2e",
                Location = new Point(20, 90),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.DarkRed
            };

            metadataGroupBox.Controls.AddRange(new Control[] 
            { 
                dataQualityLabel, dataQualityControl,
                verificationLabel, verificationStatusControl,
                notesLabel, notesControl,
                totalEmissionsLabel
            });
        }

        /// <summary>
        /// Sets up action buttons
        /// </summary>
        private void SetupActionButtons()
        {
            saveButton = new Button
            {
                Text = "Save",
                Location = new Point(530, 620),
                Size = new Size(80, 30),
                UseVisualStyleBackColor = true
            };
            saveButton.Click += SaveButton_Click;

            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(620, 620),
                Size = new Size(80, 30),
                UseVisualStyleBackColor = true
            };
            cancelButton.Click += (s, e) => Close();

            clearButton = new Button
            {
                Text = "Clear",
                Location = new Point(440, 620),
                Size = new Size(80, 30),
                UseVisualStyleBackColor = true
            };
            clearButton.Click += ClearButton_Click;

            // Wire up value changed events for real-time total calculation
            scope1EmissionsControl.ValueChanged += CalculateTotal;
            scope2MarketBasedControl.ValueChanged += CalculateTotal;
            scope3EmissionsControl.ValueChanged += CalculateTotal;
        }

        /// <summary>
        /// Loads a new empty record
        /// </summary>
        private void LoadNewRecord()
        {
            currentRecord = new EmissionRecord();
            isEditMode = false;
            Text = "New Emissions Record - " + Text;
        }

        /// <summary>
        /// Loads an existing record into the form
        /// </summary>
        private void LoadRecord()
        {
            if (currentRecord == null) return;

            reportingYearControl.Value = currentRecord.ReportingYear;
            scope1EmissionsControl.Value = currentRecord.Scope1Emissions;
            scope2LocationBasedControl.Value = currentRecord.Scope2LocationBased;
            scope2MarketBasedControl.Value = currentRecord.Scope2MarketBased;
            scope3EmissionsControl.Value = currentRecord.Scope3Emissions;
            
            dataQualityControl.Text = currentRecord.DataQuality;
            verificationStatusControl.Text = currentRecord.VerificationStatus;
            notesControl.Text = currentRecord.Notes;

            CalculateTotal(null, null);
            Text = $"Edit Emissions Record ({currentRecord.ReportingYear}) - {Text}";
        }

        /// <summary>
        /// Calculates and displays total emissions
        /// </summary>
        private void CalculateTotal(object sender, EventArgs e)
        {
            decimal total = scope1EmissionsControl.Value + scope2MarketBasedControl.Value + scope3EmissionsControl.Value;
            totalEmissionsLabel.Text = $"Total Emissions: {total:N2} tCO2e";
        }

        /// <summary>
        /// Handles the save button click event
        /// </summary>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateInput())
                {
                    SaveRecord();
                    MessageBox.Show("Emissions data saved successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the clear button click event
        /// </summary>
        private void ClearButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear all data?", "Confirm", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ClearForm();
            }
        }

        /// <summary>
        /// Validates form input
        /// </summary>
        private bool ValidateInput()
        {
            if (reportingYearControl.Value < 2020)
            {
                MessageBox.Show("Please select a valid reporting year.", "Validation Error");
                return false;
            }

            if (scope1EmissionsControl.Value < 0 || scope2LocationBasedControl.Value < 0 || 
                scope2MarketBasedControl.Value < 0 || scope3EmissionsControl.Value < 0)
            {
                MessageBox.Show("Emissions values cannot be negative.", "Validation Error");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Saves the record to the database
        /// </summary>
        private void SaveRecord()
        {
            currentRecord.ReportingYear = (int)reportingYearControl.Value;
            currentRecord.Scope1Emissions = scope1EmissionsControl.Value;
            currentRecord.Scope2LocationBased = scope2LocationBasedControl.Value;
            currentRecord.Scope2MarketBased = scope2MarketBasedControl.Value;
            currentRecord.Scope3Emissions = scope3EmissionsControl.Value;
            currentRecord.DataQuality = dataQualityControl.Text;
            currentRecord.VerificationStatus = verificationStatusControl.Text;
            currentRecord.Notes = notesControl.Text;
            currentRecord.ModifiedDate = DateTime.Now;
            currentRecord.CreatedBy = "Current User"; // TODO: Implement proper user management
        }

        /// <summary>
        /// Clears all form controls
        /// </summary>
        private void ClearForm()
        {
            scope1EmissionsControl.Value = 0;
            scope2LocationBasedControl.Value = 0;
            scope2MarketBasedControl.Value = 0;
            scope3EmissionsControl.Value = 0;
            dataQualityControl.SelectedIndex = -1;
            verificationStatusControl.SelectedIndex = -1;
            notesControl.Clear();
            CalculateTotal(null, null);
        }
    }
}