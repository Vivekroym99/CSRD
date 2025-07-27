using System;
using System.Data.SQLite;
using System.IO;

namespace CSRDReporting.DataAccess
{
    /// <summary>
    /// Handles database initialization and schema creation for the CSRD reporting application
    /// Creates and manages SQLite database with ESRS-compliant tables
    /// </summary>
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Default database file name
        /// </summary>
        private const string DatabaseFileName = "CSRDReporting.db";

        /// <summary>
        /// Gets the connection string for the SQLite database
        /// </summary>
        public static string ConnectionString => $"Data Source={GetDatabasePath()};Version=3;";

        /// <summary>
        /// Gets the full path to the database file
        /// </summary>
        /// <returns>Full path to the database file</returns>
        private static string GetDatabasePath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appDataPath, "CSRDReporting");
            
            if (!Directory.Exists(appFolder))
                Directory.CreateDirectory(appFolder);
                
            return Path.Combine(appFolder, DatabaseFileName);
        }

        /// <summary>
        /// Initializes the database and creates tables if they don't exist
        /// </summary>
        public static void InitializeDatabase()
        {
            try
            {
                using var connection = new SQLiteConnection(ConnectionString);
                connection.Open();

                CreateTables(connection);
                
                Console.WriteLine("Database initialized successfully.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to initialize database: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates all required tables for ESRS reporting
        /// </summary>
        /// <param name="connection">SQLite connection</param>
        private static void CreateTables(SQLiteConnection connection)
        {
            CreateEmissionRecordsTable(connection);
            CreateEnergyConsumptionTable(connection);
            CreateWorkforceDiversityTable(connection);
            CreateMaterialityAssessmentTable(connection);
            CreateUsersTable(connection);
        }

        /// <summary>
        /// Creates the EmissionRecords table for ESRS E1 data
        /// </summary>
        private static void CreateEmissionRecordsTable(SQLiteConnection connection)
        {
            string sql = @"
                CREATE TABLE IF NOT EXISTS EmissionRecords (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ReportingYear INTEGER NOT NULL,
                    Scope1Emissions DECIMAL(18,6) NOT NULL DEFAULT 0,
                    Scope2LocationBased DECIMAL(18,6) NOT NULL DEFAULT 0,
                    Scope2MarketBased DECIMAL(18,6) NOT NULL DEFAULT 0,
                    Scope3Emissions DECIMAL(18,6) NOT NULL DEFAULT 0,
                    DataQuality TEXT,
                    VerificationStatus TEXT DEFAULT 'Unverified',
                    Notes TEXT,
                    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    ModifiedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    CreatedBy TEXT
                );";

            using var command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates the EnergyConsumption table for ESRS E1 energy data
        /// </summary>
        private static void CreateEnergyConsumptionTable(SQLiteConnection connection)
        {
            string sql = @"
                CREATE TABLE IF NOT EXISTS EnergyConsumption (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ReportingYear INTEGER NOT NULL,
                    EnergySource TEXT NOT NULL,
                    ConsumptionMWh DECIMAL(18,6) NOT NULL DEFAULT 0,
                    IsRenewable BOOLEAN NOT NULL DEFAULT 0,
                    GridElectricity DECIMAL(18,6) DEFAULT 0,
                    SelfGeneratedRenewable DECIMAL(18,6) DEFAULT 0,
                    PurchasedRenewableCertificates DECIMAL(18,6) DEFAULT 0,
                    EnergyIntensity DECIMAL(18,6),
                    Methodology TEXT,
                    Notes TEXT,
                    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    ModifiedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    CreatedBy TEXT
                );";

            using var command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates the WorkforceDiversity table for ESRS S1 data
        /// </summary>
        private static void CreateWorkforceDiversityTable(SQLiteConnection connection)
        {
            string sql = @"
                CREATE TABLE IF NOT EXISTS WorkforceDiversity (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ReportingYear INTEGER NOT NULL,
                    EmployeeCategory TEXT NOT NULL,
                    TotalEmployees INTEGER NOT NULL DEFAULT 0,
                    FemaleEmployees INTEGER NOT NULL DEFAULT 0,
                    MaleEmployees INTEGER NOT NULL DEFAULT 0,
                    NonBinaryEmployees INTEGER NOT NULL DEFAULT 0,
                    EmployeesUnder30 INTEGER NOT NULL DEFAULT 0,
                    Employees30To50 INTEGER NOT NULL DEFAULT 0,
                    EmployeesOver50 INTEGER NOT NULL DEFAULT 0,
                    EmployeesWithDisabilities INTEGER NOT NULL DEFAULT 0,
                    EthnicMinorityEmployees INTEGER NOT NULL DEFAULT 0,
                    AverageTrainingHours DECIMAL(10,2) NOT NULL DEFAULT 0,
                    TurnoverRate DECIMAL(5,2) NOT NULL DEFAULT 0,
                    GenderPayGap DECIMAL(5,2),
                    Notes TEXT,
                    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    ModifiedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    CreatedBy TEXT
                );";

            using var command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates the MaterialityAssessment table for double materiality data
        /// </summary>
        private static void CreateMaterialityAssessmentTable(SQLiteConnection connection)
        {
            string sql = @"
                CREATE TABLE IF NOT EXISTS MaterialityAssessment (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ReportingYear INTEGER NOT NULL,
                    SustainabilityTopic TEXT NOT NULL,
                    ESRSStandard TEXT,
                    ImpactMateriality INTEGER NOT NULL CHECK(ImpactMateriality >= 1 AND ImpactMateriality <= 5),
                    FinancialMateriality INTEGER NOT NULL CHECK(FinancialMateriality >= 1 AND FinancialMateriality <= 5),
                    TimeHorizon TEXT,
                    StakeholderGroups TEXT,
                    AssessmentMethodology TEXT,
                    IdentifiedRisks TEXT,
                    IdentifiedOpportunities TEXT,
                    SupportingEvidence TEXT,
                    ManagementResponse TEXT,
                    ReviewStatus TEXT DEFAULT 'Draft',
                    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    ModifiedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    CreatedBy TEXT
                );";

            using var command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates the Users table for basic user management
        /// </summary>
        private static void CreateUsersTable(SQLiteConnection connection)
        {
            string sql = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT UNIQUE NOT NULL,
                    PasswordHash TEXT NOT NULL,
                    FullName TEXT NOT NULL,
                    Email TEXT,
                    Role TEXT NOT NULL DEFAULT 'User',
                    IsActive BOOLEAN NOT NULL DEFAULT 1,
                    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    LastLoginDate DATETIME
                );";

            using var command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();

            // Create default admin user if no users exist
            CreateDefaultUser(connection);
        }

        /// <summary>
        /// Creates a default admin user if no users exist
        /// </summary>
        private static void CreateDefaultUser(SQLiteConnection connection)
        {
            string checkSql = "SELECT COUNT(*) FROM Users";
            using var checkCommand = new SQLiteCommand(checkSql, connection);
            long userCount = (long)checkCommand.ExecuteScalar();

            if (userCount == 0)
            {
                string insertSql = @"
                    INSERT INTO Users (Username, PasswordHash, FullName, Role)
                    VALUES ('admin', 'admin123', 'System Administrator', 'Admin')";
                
                using var insertCommand = new SQLiteCommand(insertSql, connection);
                insertCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Checks if the database exists and is accessible
        /// </summary>
        /// <returns>True if database is accessible, false otherwise</returns>
        public static bool DatabaseExists()
        {
            try
            {
                using var connection = new SQLiteConnection(ConnectionString);
                connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}