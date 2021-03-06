﻿using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using VipcoTransport.Models;
using VipcoTransport.Services.Interfaces;
using VipcoTransport.ViewModels;

namespace VipcoTransport.Services.Classes
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        #region PrivateMembers
        private DbSet<TEntity> entities;
        private ApplicationContext Context;
        string errorMessage = string.Empty;
        #endregion

        #region Constructor
        /// <summary>
        /// The contructor requires an open DataContext to work with
        /// </summary>
        /// <param name="context">An open DataContext</param>

        public Repository(ApplicationContext context)
        {
            this.Context = context;
            entities = context.Set<TEntity>();
        }
        #endregion

        /// <summary>
        /// Returns a single object with a primary key of the provided id
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="id">The primary key of the object to fetch</param>
        /// <returns>A single object with the provided primary key or null</returns>
        public TEntity Get(int id)
        {
            return this.entities.Find(id);
        }
        public TEntity Get(string id)
        {
            return this.entities.Find(id);
        }
        /// <summary>
        /// Returns a single object with a primary key of the provided id
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="id">The primary key of the object to fetch</param>
        /// <returns>A single object with the provided primary key or null</returns>
        public async Task<TEntity> GetAsync(int id)
        {
            return await this.Context.Set<TEntity>().FindAsync(id);
            //return await this.entities.FindAsync(id);
        }
        /// <summary>
        /// Returns a single object with a primary key of the provided id
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="key">The primary key of the object to fetch</param>
        /// <returns>A single object with the provided primary key or null</returns>
        public async Task<TEntity> GetAsync(string key)
        {
            return await this.Context.Set<TEntity>().FindAsync(key);
            //return await this.entities.FindAsync(key);
        }
        /// <summary>
        /// Gets a collection of all objects in the database
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <returns>An ICollection of every object in the database</returns>
        //public ICollection<TEntity> GetAll()
        //{
        //    return this.entities.AsNoTracking().ToList();
        //}

        public IQueryable<TEntity> GetAll()
        {
            return this.entities.AsQueryable();
        }

        /// <summary>
        /// Gets a collection of all objects in the database
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <returns>An ICollection of every object in the database</returns>
        public async Task<ICollection<TEntity>> GetAllAsync()
        {
            return await this.entities.AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<TEntity>> GetAllWithRelateAsync(Expression<Func<TEntity, bool>> match = null)
        {
            var Query = this.entities.AsQueryable();
            if (match != null)
            {
                Query = Query.Where(match);
            }
            return await Query.ToListAsync();
        }

        /// <summary>
        /// Gets a collection of all objects in the database
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="relates">List of linq expression relate to find one or more relate</param>
        /// <returns>An ICollection of every object in the database</returns>
        public async Task<ICollection<TEntity>> GetAllWithIncludeAsync
            (List<Expression<Func<TEntity, object>>> relates)
        {
            var Query = this.entities.AsQueryable();

            foreach (var relate in relates)
                Query = Query.Include(relate);

            return await Query.AsNoTracking().ToListAsync();
        }
        /// <summary>
        /// Returns a single object which matches the provided expression
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="match">A Linq expression filter to find a single result</param>
        /// <returns>A single object which matches the expression filter.
        /// If more than one object is found or if zero are found, null is returned</returns>
        public TEntity Find(Expression<Func<TEntity, bool>> match)
        {
            return this.entities.AsNoTracking().SingleOrDefault(match);
        }
        /// <summary>
        /// Returns a single object which matches the provided expression
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="match">A Linq expression filter to find a single result</param>
        /// <returns>A single object which matches the expression filter.
        /// If more than one object is found or if zero are found, null is returned</returns>
        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match)
        {
            return await this.entities.AsNoTracking().SingleOrDefaultAsync(match);
        }
        /// <summary>
        /// Returns a collection of objects which match the provided expression
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="match">A linq expression filter to find one or more results</param>
        /// <returns>An ICollection of object which match the expression filter</returns>
        public ICollection<TEntity> FindAll(Expression<Func<TEntity, bool>> match)
        {
            return this.entities.Where(match).AsNoTracking().ToList();
        }
        /// <summary>
        /// Returns a collection of objects which match the provided expression
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="match">A linq expression filter to find one or more results</param>
        /// <returns>An ICollection of object which match the expression filter</returns>
        public async Task<ICollection<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match)
        {
            return await this.entities.Where(match).AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Returns a collection of objects which match the provided expression
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="match">A linq expression filter to find one or more results</param>
        /// <param name="relates">List of linq expression relate to find one or more relate</param>
        /// <returns>An ICollection of object which match the expression filter</returns>
        public async Task<ICollection<TEntity>> FindAllWithIncludeAsync
            (Expression<Func<TEntity, bool>> match, List<Expression<Func<TEntity, object>>> relates)
        {
            var Query = this.entities.Where(match);

            foreach (var relate in relates)
                Query = Query.Include(relate);

            return await Query.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Returns a collection of objects which match the provided expression
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="match">A linq expression filter to find one or more results</param>
        /// <param name="relates">List of linq expression relate to find one or more relate</param>
        /// <returns>An ICollection of object which match the expression filter</returns>
        public async Task<ICollection<TEntity>> FindAllWithLazyLoadAsync
            (Expression<Func<TEntity, bool>> match,
            List<Expression<Func<TEntity, object>>> relates,
            int Skip, int Row,
            Expression<Func<TEntity, string>> order = null,
            Expression<Func<TEntity, string>> orderDesc = null)
        {
            try
            {
                var Query = this.entities.AsQueryable();

                if (match != null)
                    Query = Query.Where(match);
                //Relate
                if (relates != null)
                {
                    foreach (var relate in relates)
                        Query = Query.Include(relate);
                }
                //Order
                if (order != null)
                    Query = Query.OrderBy(order);
                else if (orderDesc != null)
                    Query = Query.OrderByDescending(orderDesc);
                //Skip Take
                Query = Query.Skip(Skip).Take(Row);

                return await Query.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
                return null;
            }
        }
        public ICollection<TEntity> FindAllWithLazyLoad
           (Expression<Func<TEntity, bool>> match,
           List<Expression<Func<TEntity, object>>> relates,
           int Skip, int Row,
           Expression<Func<TEntity, string>> order = null,
           Expression<Func<TEntity, string>> orderDesc = null)
        {
            var Query = this.entities.Where(match);
            //Relate
            foreach (var relate in relates)
                Query = Query.Include(relate);
            //Order
            if (order != null)
                Query = Query.OrderBy(order);
            else if (orderDesc != null)
                Query = Query.OrderByDescending(orderDesc);
            //Skip Take
            Query = Query.Skip(Skip).Take(Row);

            return Query.AsNoTracking().ToList();
        }
        /// <summary>
        /// Inserts a single object to the database and commits the change
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="t">The object to insert</param>
        /// <returns>The resulting object including its primary key after the insert</returns>
        public TEntity Add(TEntity t)
        {
            this.entities.Add(t);
            this.Context.SaveChanges();
            return t;
        }
        /// <summary>
        /// Inserts a single object to the database and commits the change
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="t">The object to insert</param>
        /// <returns>The resulting object including its primary key after the insert</returns>
        public async Task<TEntity> AddAsync(TEntity t)
        {
            this.entities.Add(t);
            await this.Context.SaveChangesAsync();
            return t;
        }
        /// <summary>
        /// Inserts a collection of objects into the database and commits the changes
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="tList">An IEnumerable list of objects to insert</param>
        /// <returns>The IEnumerable resulting list of inserted objects including the primary keys</returns>
        public IEnumerable<TEntity> AddAll(IEnumerable<TEntity> tList)
        {
            this.entities.AddRange(tList);
            this.Context.SaveChanges();
            return tList;
        }
        /// <summary>
        /// Inserts a collection of objects into the database and commits the changes
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="tList">An IEnumerable list of objects to insert</param>
        /// <returns>The IEnumerable resulting list of inserted objects including the primary keys</returns>
        public async Task<IEnumerable<TEntity>> AddAllAsync(IEnumerable<TEntity> tList)
        {
            this.entities.AddRange(tList);
            await this.Context.SaveChangesAsync();
            return tList;
        }
        /// <summary>
        /// Updates a single object based on the provided primary key and commits the change
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="updated">The updated object to apply to the database</param>
        /// <param name="key">The primary key of the object to update</param>
        /// <returns>The resulting updated object</returns>
        public TEntity Update(TEntity updated, int key)
        {
            if (updated == null)
                return null;

            TEntity existing = this.entities.Find(key);
            if (existing != null)
            {
                this.Context.Entry(existing).CurrentValues.SetValues(updated);
                this.Context.SaveChanges();
            }
            return existing;
        }
        /// <summary>
        /// Updates a single object based on the provided primary key and commits the change
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="updated">The updated object to apply to the database</param>
        /// <param name="key">The primary key of the object to update</param>
        /// <returns>The resulting updated object</returns>
        public TEntity Update(TEntity updated, string key)
        {
            if (updated == null)
                return null;

            TEntity existing = this.entities.Find(key);
            if (existing != null)
            {
                this.Context.Entry(existing).CurrentValues.SetValues(updated);
                this.Context.SaveChanges();
            }
            return existing;
        }

        /// <summary>
        /// Updates a single object based on the provided primary key and commits the change
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="updated">The updated object to apply to the database</param>
        /// <param name="key">The primary key of the object to update</param>
        /// <returns>The resulting updated object</returns>
        public async Task<TEntity> UpdateAsync(TEntity updated, int key)
        {
            if (updated == null)
                return null;

            TEntity existing = await this.entities.FindAsync(key);
            if (existing != null)
            {
                this.Context.Entry(existing).CurrentValues.SetValues(updated);
                await this.Context.SaveChangesAsync();
            }
            return existing;
        }
        /// <summary>
        /// Updates a single object based on the provided primary key and commits the change
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="updated">The updated object to apply to the database</param>
        /// <param name="key">The primary key of the object to update</param>
        /// <returns>The resulting updated object</returns>
        public async Task<TEntity> UpdateAsync(TEntity updated, string key)
        {
            if (updated == null)
                return null;

            TEntity existing = await this.entities.FindAsync(key);
            if (existing != null)
            {
                this.Context.Entry(existing).CurrentValues.SetValues(updated);
                await this.Context.SaveChangesAsync();
            }
            return existing;
        }
        /// <summary>
        /// Deletes a single object from the database and commits the change
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <param name="t">The object to delete</param>
        public void Delete(int key)
        {
            TEntity existing = this.entities.Find(key);
            if (existing != null)
            {
                this.entities.Remove(existing);
                this.Context.SaveChanges();
            }
        }
        /// <summary>
        /// Deletes a single object from the database and commits the change
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="key">The primary key of the object to delete</param>

        public async Task<int> DeleteAsync(int key)
        {
            TEntity existing = await this.entities.FindAsync(key);
            if (existing != null)
            {
                this.entities.Remove(existing);
                return await this.Context.SaveChangesAsync();
            }
            return 0;
        }

        /// <summary>
        /// Deletes a single object from the database and commits the change
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <param name="key">The primary key of the object to delete</param>
        public async Task<int> DeleteAsync(string key)
        {
            TEntity existing = await this.entities.FindAsync(key);
            if (existing != null)
            {
                this.entities.Remove(existing);
                return await this.Context.SaveChangesAsync();
            }
            return 0;
        }

        /// <summary>
        /// Gets the count of the number of objects in the databse
        /// </summary>
        /// <remarks>Synchronous</remarks>
        /// <returns>The count of the number of objects</returns>
        public int Count()
        {
            return this.entities.Count();
        }

        public int CountWithMatch(Expression<Func<TEntity, bool>> match)
        {
            var Query = this.entities.AsQueryable();

            if (match != null)
                Query = Query.Where(match);

            return Query.Count();
        }
        /// <summary>
        /// Gets the count of the number of objects in the databse
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        /// <returns>The count of the number of objects</returns>
        public async Task<int> CountAsync()
        {
            return await this.entities.CountAsync();
        }
        /// <summary>
        /// Gets the count of the number of objects in the databse
        /// </summary>
        /// <remarks>Asynchronous</remarks>
        ///  /// <param name="match">A linq expression filter to find one or more results</param>
        /// <returns>The count of the number of objects</returns>
        public async Task<int> CountWithMatchAsync(Expression<Func<TEntity, bool>> match)
        {
            return await this.entities.Where(match).CountAsync();
        }
    }
}
