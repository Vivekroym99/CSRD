using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSRDReporting.DataAccess
{
    /// <summary>
    /// Generic repository interface for data access operations
    /// Provides standard CRUD operations for all entities
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Gets all entities
        /// </summary>
        /// <returns>List of all entities</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Gets an entity by its ID
        /// </summary>
        /// <param name="id">Entity ID</param>
        /// <returns>Entity or null if not found</returns>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new entity
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <returns>Added entity with generated ID</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <returns>Updated entity</returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Deletes an entity by ID
        /// </summary>
        /// <param name="id">Entity ID to delete</param>
        /// <returns>True if deleted, false if not found</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Gets entities by reporting year
        /// </summary>
        /// <param name="year">Reporting year</param>
        /// <returns>List of entities for the specified year</returns>
        Task<IEnumerable<T>> GetByYearAsync(int year);

        /// <summary>
        /// Checks if any entity exists for the specified year
        /// </summary>
        /// <param name="year">Reporting year</param>
        /// <returns>True if entities exist for the year</returns>
        Task<bool> ExistsForYearAsync(int year);
    }
}