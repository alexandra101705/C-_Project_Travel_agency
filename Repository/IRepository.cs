using System;
using System.Collections.Generic;
using Agentie_turism_transport_csharp.Domain;

namespace Agentie_turism_transport_csharp.Repository
{
    /// <summary>
    /// CRUD operations repository interface
    /// </summary>
    /// <typeparam name="ID">Type of the entity's ID</typeparam>
    /// <typeparam name="E">Type of the entities stored in the repository</typeparam>
    public interface IRepository<ID, E> where E : Entity<ID>
    {
        /// <summary>
        /// Finds an entity by its ID
        /// </summary>
        /// <param name="id">ID of the entity to be returned, must not be null</param>
        /// <returns>An optional entity with the given ID</returns>
        /// <exception cref="ArgumentException">Thrown if id is null</exception>
        E FindOne(ID id);

        /// <summary>
        /// Retrieves all entities
        /// </summary>
        /// <returns>An enumerable collection of entities</returns>
        IEnumerable<E> FindAll();

        /// <summary>
        /// Saves an entity
        /// </summary>
        /// <param name="entity">Entity to be saved, must not be null</param>
        /// <returns>Null if the entity was saved, or the entity if the ID already exists</returns>
        /// <exception cref="ArgumentException">Thrown if entity is null</exception>
        void Save(E entity);

        /// <summary>
        /// Deletes an entity by its ID
        /// </summary>
        /// <param name="id">ID of the entity to be deleted, must not be null</param>
        /// <returns>Null if no entity was found, or the deleted entity</returns>
        /// <exception cref="ArgumentException">Thrown if id is null</exception>
        void Delete(ID id);

        /// <summary>
        /// Updates an entity
        /// </summary>
        /// <param name="entity">Entity to be updated, must not be null</param>
        /// <returns>Null if the entity was updated, or the entity if the ID does not exist</returns>
        /// <exception cref="ArgumentException">Thrown if entity is null</exception>
        void Update(E entity);
    }
}
