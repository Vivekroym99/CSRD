using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using CSRDReporting.Models;
using Dapper;

namespace CSRDReporting.DataAccess
{
    /// <summary>
    /// Repository for managing EmissionRecord data access operations
    /// Implements CRUD operations for GHG emissions data
    /// </summary>
    public class EmissionsRepository : IRepository<EmissionRecord>
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the EmissionsRepository
        /// </summary>
        public EmissionsRepository()
        {
            _connectionString = DatabaseInitializer.ConnectionString;
        }

        /// <summary>
        /// Gets all emission records
        /// </summary>
        public async Task<IEnumerable<EmissionRecord>> GetAllAsync()
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT Id, ReportingYear, Scope1Emissions, Scope2LocationBased, 
                       Scope2MarketBased, Scope3Emissions, DataQuality, 
                       VerificationStatus, Notes, CreatedDate, ModifiedDate, CreatedBy
                FROM EmissionRecords 
                ORDER BY ReportingYear DESC, ModifiedDate DESC";

            return await connection.QueryAsync<EmissionRecord>(sql);
        }

        /// <summary>
        /// Gets an emission record by ID
        /// </summary>
        public async Task<EmissionRecord?> GetByIdAsync(int id)
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT Id, ReportingYear, Scope1Emissions, Scope2LocationBased, 
                       Scope2MarketBased, Scope3Emissions, DataQuality, 
                       VerificationStatus, Notes, CreatedDate, ModifiedDate, CreatedBy
                FROM EmissionRecords 
                WHERE Id = @Id";

            return await connection.QuerySingleOrDefaultAsync<EmissionRecord>(sql, new { Id = id });
        }

        /// <summary>
        /// Gets emission records by reporting year
        /// </summary>
        public async Task<IEnumerable<EmissionRecord>> GetByYearAsync(int year)
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT Id, ReportingYear, Scope1Emissions, Scope2LocationBased, 
                       Scope2MarketBased, Scope3Emissions, DataQuality, 
                       VerificationStatus, Notes, CreatedDate, ModifiedDate, CreatedBy
                FROM EmissionRecords 
                WHERE ReportingYear = @Year
                ORDER BY ModifiedDate DESC";

            return await connection.QueryAsync<EmissionRecord>(sql, new { Year = year });
        }

        /// <summary>
        /// Checks if emission records exist for the specified year
        /// </summary>
        public async Task<bool> ExistsForYearAsync(int year)
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = "SELECT COUNT(*) FROM EmissionRecords WHERE ReportingYear = @Year";
            
            var count = await connection.QuerySingleAsync<int>(sql, new { Year = year });
            return count > 0;
        }

        /// <summary>
        /// Adds a new emission record
        /// </summary>
        public async Task<EmissionRecord> AddAsync(EmissionRecord entity)
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                INSERT INTO EmissionRecords (
                    ReportingYear, Scope1Emissions, Scope2LocationBased, 
                    Scope2MarketBased, Scope3Emissions, DataQuality, 
                    VerificationStatus, Notes, CreatedDate, ModifiedDate, CreatedBy
                ) VALUES (
                    @ReportingYear, @Scope1Emissions, @Scope2LocationBased, 
                    @Scope2MarketBased, @Scope3Emissions, @DataQuality, 
                    @VerificationStatus, @Notes, @CreatedDate, @ModifiedDate, @CreatedBy
                );
                SELECT last_insert_rowid();";

            entity.CreatedDate = DateTime.Now;
            entity.ModifiedDate = DateTime.Now;

            var id = await connection.QuerySingleAsync<int>(sql, entity);
            entity.Id = id;

            return entity;
        }

        /// <summary>
        /// Updates an existing emission record
        /// </summary>
        public async Task<EmissionRecord> UpdateAsync(EmissionRecord entity)
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                UPDATE EmissionRecords SET
                    ReportingYear = @ReportingYear,
                    Scope1Emissions = @Scope1Emissions,
                    Scope2LocationBased = @Scope2LocationBased,
                    Scope2MarketBased = @Scope2MarketBased,
                    Scope3Emissions = @Scope3Emissions,
                    DataQuality = @DataQuality,
                    VerificationStatus = @VerificationStatus,
                    Notes = @Notes,
                    ModifiedDate = @ModifiedDate,
                    CreatedBy = @CreatedBy
                WHERE Id = @Id";

            entity.ModifiedDate = DateTime.Now;

            var rowsAffected = await connection.ExecuteAsync(sql, entity);
            
            if (rowsAffected == 0)
                throw new InvalidOperationException($"Emission record with ID {entity.Id} not found.");

            return entity;
        }

        /// <summary>
        /// Deletes an emission record by ID
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = "DELETE FROM EmissionRecords WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }

        /// <summary>
        /// Gets emission records for a specific reporting year range
        /// </summary>
        /// <param name="startYear">Start year (inclusive)</param>
        /// <param name="endYear">End year (inclusive)</param>
        /// <returns>Emission records within the specified year range</returns>
        public async Task<IEnumerable<EmissionRecord>> GetByYearRangeAsync(int startYear, int endYear)
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT Id, ReportingYear, Scope1Emissions, Scope2LocationBased, 
                       Scope2MarketBased, Scope3Emissions, DataQuality, 
                       VerificationStatus, Notes, CreatedDate, ModifiedDate, CreatedBy
                FROM EmissionRecords 
                WHERE ReportingYear BETWEEN @StartYear AND @EndYear
                ORDER BY ReportingYear, ModifiedDate DESC";

            return await connection.QueryAsync<EmissionRecord>(sql, new { StartYear = startYear, EndYear = endYear });
        }

        /// <summary>
        /// Gets emission trends by calculating year-over-year changes
        /// </summary>
        /// <returns>Dictionary with years as keys and total emissions as values</returns>
        public async Task<Dictionary<int, decimal>> GetEmissionTrendsAsync()
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT ReportingYear, 
                       SUM(Scope1Emissions + Scope2MarketBased + Scope3Emissions) as TotalEmissions
                FROM EmissionRecords 
                GROUP BY ReportingYear
                ORDER BY ReportingYear";

            var results = await connection.QueryAsync<(int Year, decimal Total)>(sql);
            
            return results.ToDictionary(r => r.Year, r => r.Total);
        }

        /// <summary>
        /// Gets verification statistics for emissions data
        /// </summary>
        /// <returns>Dictionary with verification status and count</returns>
        public async Task<Dictionary<string, int>> GetVerificationStatisticsAsync()
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();

            const string sql = @"
                SELECT VerificationStatus, COUNT(*) as Count
                FROM EmissionRecords 
                WHERE VerificationStatus IS NOT NULL AND VerificationStatus != ''
                GROUP BY VerificationStatus";

            var results = await connection.QueryAsync<(string Status, int Count)>(sql);
            
            return results.ToDictionary(r => r.Status, r => r.Count);
        }
    }
}