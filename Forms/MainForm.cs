using System;
using System.Drawing;
using System.Windows.Forms;
using CSRDReporting.DataAccess;

namespace CSRDReporting.Forms
{
    /// <summary>
    /// Main application form providing navigation to all CSRD reporting modules
    /// Implements tabbed interface for different ESRS reporting areas
    /// </summary>
    public partial class MainForm : Form
    {
        private TabControl mainTabControl;
        private MenuStrip mainMenuStrip;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;

        /// <summary>
        /// Initializes the main form with navigation structure
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            InitializeDatabase();
            SetupMainInterface();
        }

        /// <summary>
        /// Initializes the database on application startup
        /// </summary>
        private void InitializeDatabase()
        {
            try
            {
                DatabaseInitializer.InitializeDatabase();
                SetStatusMessage("Database initialized successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize database: {ex.Message}", 
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatusMessage("Database initialization failed");
            }
        }

        /// <summary>
        /// Sets up the main user interface components
        /// </summary>
        private void SetupMainInterface()
        {
            SetupForm();
            SetupMenuStrip();
            SetupTabControl();
            SetupStatusStrip();
        }

        /// <summary>
        /// Configures the main form properties
        /// </summary>
        private void SetupForm()
        {
            Text = "CSRD & ESG Reporting System v1.0";
            Size = new Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1000, 600);
            Icon = SystemIcons.Application;
        }

        /// <summary>
        /// Sets up the main menu strip with application commands
        /// </summary>
        private void SetupMenuStrip()
        {
            mainMenuStrip = new MenuStrip();

            // File Menu
            var fileMenu = new ToolStripMenuItem("&File");
            fileMenu.DropDownItems.Add("&New Report", null, (s, e) => NewReport());
            fileMenu.DropDownItems.Add("&Open Report", null, (s, e) => OpenReport());
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add("&Export Data", null, (s, e) => ExportData());
            fileMenu.DropDownItems.Add("&Import Data", null, (s, e) => ImportData());
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add("E&xit", null, (s, e) => Close());

            // Data Menu
            var dataMenu = new ToolStripMenuItem("&Data");
            dataMenu.DropDownItems.Add("&Emissions Data", null, (s, e) => ShowEmissionsForm());
            dataMenu.DropDownItems.Add("&Energy Consumption", null, (s, e) => ShowEnergyForm());
            dataMenu.DropDownItems.Add("&Workforce Data", null, (s, e) => ShowWorkforceForm());
            dataMenu.DropDownItems.Add("&Materiality Assessment", null, (s, e) => ShowMaterialityForm());

            // Reports Menu
            var reportsMenu = new ToolStripMenuItem("&Reports");
            reportsMenu.DropDownItems.Add("&ESRS E1 Climate Report", null, (s, e) => GenerateClimateReport());
            reportsMenu.DropDownItems.Add("&ESRS S1 Workforce Report", null, (s, e) => GenerateWorkforceReport());
            reportsMenu.DropDownItems.Add("&Complete CSRD Report", null, (s, e) => GenerateFullReport());
            reportsMenu.DropDownItems.Add(new ToolStripSeparator());
            reportsMenu.DropDownItems.Add("&Compliance Checklist", null, (s, e) => ShowComplianceChecklist());

            // Tools Menu
            var toolsMenu = new ToolStripMenuItem("&Tools");
            toolsMenu.DropDownItems.Add("&Data Validation", null, (s, e) => ValidateData());
            toolsMenu.DropDownItems.Add("&Backup Database", null, (s, e) => BackupDatabase());
            toolsMenu.DropDownItems.Add("&Settings", null, (s, e) => ShowSettings());

            // Help Menu
            var helpMenu = new ToolStripMenuItem("&Help");
            helpMenu.DropDownItems.Add("&User Guide", null, (s, e) => ShowUserGuide());
            helpMenu.DropDownItems.Add("&ESRS Standards", null, (s, e) => ShowESRSStandards());
            helpMenu.DropDownItems.Add(new ToolStripSeparator());
            helpMenu.DropDownItems.Add("&About", null, (s, e) => ShowAbout());

            mainMenuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, dataMenu, reportsMenu, toolsMenu, helpMenu });
            MainMenuStrip = mainMenuStrip;
            Controls.Add(mainMenuStrip);
        }

        /// <summary>
        /// Sets up the main tab control for different reporting modules
        /// </summary>
        private void SetupTabControl()
        {
            mainTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F),
                Padding = new Point(12, 6)
            };

            // Dashboard Tab
            var dashboardTab = new TabPage("Dashboard");
            dashboardTab.Controls.Add(CreateDashboardPanel());
            mainTabControl.TabPages.Add(dashboardTab);

            // Environmental Data Tab
            var environmentalTab = new TabPage("Environmental (ESRS E)");
            environmentalTab.Controls.Add(CreateEnvironmentalPanel());
            mainTabControl.TabPages.Add(environmentalTab);

            // Social Data Tab
            var socialTab = new TabPage("Social (ESRS S)");
            socialTab.Controls.Add(CreateSocialPanel());
            mainTabControl.TabPages.Add(socialTab);

            // Governance Data Tab
            var governanceTab = new TabPage("Governance (ESRS G)");
            governanceTab.Controls.Add(CreateGovernancePanel());
            mainTabControl.TabPages.Add(governanceTab);

            // Materiality Assessment Tab
            var materialityTab = new TabPage("Materiality Assessment");
            materialityTab.Controls.Add(CreateMaterialityPanel());
            mainTabControl.TabPages.Add(materialityTab);

            // Reports Tab
            var reportsTab = new TabPage("Reports & Compliance");
            reportsTab.Controls.Add(CreateReportsPanel());
            mainTabControl.TabPages.Add(reportsTab);

            Controls.Add(mainTabControl);
        }

        /// <summary>
        /// Sets up the status strip at the bottom of the form
        /// </summary>
        private void SetupStatusStrip()
        {
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel("Ready")
            {
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var dateLabel = new ToolStripStatusLabel($"Current Date: {DateTime.Now:yyyy-MM-dd}");
            var userLabel = new ToolStripStatusLabel("User: System Admin");

            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel, dateLabel, userLabel });
            Controls.Add(statusStrip);
        }

        /// <summary>
        /// Creates the dashboard panel with key metrics overview
        /// </summary>
        private Panel CreateDashboardPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            
            var titleLabel = new Label
            {
                Text = "CSRD & ESG Reporting Dashboard",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(500, 40)
            };

            var infoLabel = new Label
            {
                Text = "Welcome to the Corporate Sustainability Reporting Directive (CSRD) compliance system.\n\n" +
                       "This application helps you collect, manage, and report sustainability data according to " +
                       "European Sustainability Reporting Standards (ESRS).\n\n" +
                       "Use the tabs above to navigate between different reporting areas:\n" +
                       "• Environmental (ESRS E): Climate, pollution, water, biodiversity, circular economy\n" +
                       "• Social (ESRS S): Workforce, value chain, communities, consumers\n" +
                       "• Governance (ESRS G): Business conduct and governance practices\n" +
                       "• Materiality Assessment: Double materiality analysis\n" +
                       "• Reports & Compliance: Generate reports and check compliance",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 80),
                Size = new Size(700, 300),
                AutoSize = false
            };

            panel.Controls.AddRange(new Control[] { titleLabel, infoLabel });
            return panel;
        }

        /// <summary>
        /// Creates the environmental data panel for ESRS E standards
        /// </summary>
        private Panel CreateEnvironmentalPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            
            var titleLabel = new Label
            {
                Text = "Environmental Data (ESRS E Standards)",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(500, 30)
            };

            var emissionsButton = new Button
            {
                Text = "ESRS E1: Climate Change & Emissions",
                Size = new Size(300, 40),
                Location = new Point(20, 70),
                UseVisualStyleBackColor = true
            };
            emissionsButton.Click += (s, e) => ShowEmissionsForm();

            var energyButton = new Button
            {
                Text = "ESRS E1: Energy Consumption",
                Size = new Size(300, 40),
                Location = new Point(20, 120),
                UseVisualStyleBackColor = true
            };
            energyButton.Click += (s, e) => ShowEnergyForm();

            panel.Controls.AddRange(new Control[] { titleLabel, emissionsButton, energyButton });
            return panel;
        }

        /// <summary>
        /// Creates the social data panel for ESRS S standards
        /// </summary>
        private Panel CreateSocialPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            
            var titleLabel = new Label
            {
                Text = "Social Data (ESRS S Standards)",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(500, 30)
            };

            var workforceButton = new Button
            {
                Text = "ESRS S1: Own Workforce",
                Size = new Size(300, 40),
                Location = new Point(20, 70),
                UseVisualStyleBackColor = true
            };
            workforceButton.Click += (s, e) => ShowWorkforceForm();

            panel.Controls.AddRange(new Control[] { titleLabel, workforceButton });
            return panel;
        }

        /// <summary>
        /// Creates the governance data panel for ESRS G standards
        /// </summary>
        private Panel CreateGovernancePanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            
            var titleLabel = new Label
            {
                Text = "Governance Data (ESRS G Standards)",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(500, 30)
            };

            var infoLabel = new Label
            {
                Text = "ESRS G1: Business Conduct data entry forms will be available in future versions.",
                Location = new Point(20, 70),
                Size = new Size(600, 30)
            };

            panel.Controls.AddRange(new Control[] { titleLabel, infoLabel });
            return panel;
        }

        /// <summary>
        /// Creates the materiality assessment panel
        /// </summary>
        private Panel CreateMaterialityPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            
            var titleLabel = new Label
            {
                Text = "Double Materiality Assessment",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(500, 30)
            };

            var materialityButton = new Button
            {
                Text = "Manage Materiality Assessments",
                Size = new Size(300, 40),
                Location = new Point(20, 70),
                UseVisualStyleBackColor = true
            };
            materialityButton.Click += (s, e) => ShowMaterialityForm();

            panel.Controls.AddRange(new Control[] { titleLabel, materialityButton });
            return panel;
        }

        /// <summary>
        /// Creates the reports and compliance panel
        /// </summary>
        private Panel CreateReportsPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            
            var titleLabel = new Label
            {
                Text = "Reports & Compliance",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(500, 30)
            };

            var complianceButton = new Button
            {
                Text = "Compliance Checklist",
                Size = new Size(200, 40),
                Location = new Point(20, 70),
                UseVisualStyleBackColor = true
            };
            complianceButton.Click += (s, e) => ShowComplianceChecklist();

            var reportButton = new Button
            {
                Text = "Generate CSRD Report",
                Size = new Size(200, 40),
                Location = new Point(240, 70),
                UseVisualStyleBackColor = true
            };
            reportButton.Click += (s, e) => GenerateFullReport();

            panel.Controls.AddRange(new Control[] { titleLabel, complianceButton, reportButton });
            return panel;
        }

        /// <summary>
        /// Sets the status message in the status bar
        /// </summary>
        private void SetStatusMessage(string message)
        {
            if (statusLabel != null)
                statusLabel.Text = message;
        }

        #region Menu Event Handlers

        private void NewReport() => MessageBox.Show("New Report functionality will be implemented.", "Information");
        private void OpenReport() => MessageBox.Show("Open Report functionality will be implemented.", "Information");
        private void ExportData() => MessageBox.Show("Export Data functionality will be implemented.", "Information");
        private void ImportData() => MessageBox.Show("Import Data functionality will be implemented.", "Information");
        
        private void ShowEmissionsForm()
        {
            var emissionsForm = new EmissionsDataForm();
            emissionsForm.ShowDialog(this);
        }
        
        private void ShowEnergyForm() => MessageBox.Show("Energy Consumption form will be implemented.", "Information");
        private void ShowWorkforceForm() => MessageBox.Show("Workforce Data form will be implemented.", "Information");
        private void ShowMaterialityForm() => MessageBox.Show("Materiality Assessment form will be implemented.", "Information");
        
        private void GenerateClimateReport() => MessageBox.Show("Climate Report generation will be implemented.", "Information");
        private void GenerateWorkforceReport() => MessageBox.Show("Workforce Report generation will be implemented.", "Information");
        private void GenerateFullReport() => MessageBox.Show("Full CSRD Report generation will be implemented.", "Information");
        private void ShowComplianceChecklist() => MessageBox.Show("Compliance Checklist will be implemented.", "Information");
        
        private void ValidateData() => MessageBox.Show("Data Validation functionality will be implemented.", "Information");
        private void BackupDatabase() => MessageBox.Show("Database Backup functionality will be implemented.", "Information");
        private void ShowSettings() => MessageBox.Show("Settings dialog will be implemented.", "Information");
        
        private void ShowUserGuide() => MessageBox.Show("User Guide will be implemented.", "Information");
        private void ShowESRSStandards() => MessageBox.Show("ESRS Standards reference will be implemented.", "Information");
        private void ShowAbout()
        {
            MessageBox.Show("CSRD & ESG Reporting System v1.0\n\n" +
                          "Corporate Sustainability Reporting Directive compliance application\n" +
                          "Supports European Sustainability Reporting Standards (ESRS)\n\n" +
                          "© 2024 - Built with .NET Windows Forms", 
                          "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
    }
}