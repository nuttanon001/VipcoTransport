using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using VipcoTransport.ViewModels;

namespace VipcoTransport.Services.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetAsync(int id);
        Task<TEntity> GetAsync(string TEntityId);
        TEntity Get(string TEntityId);
        TEntity Get(int id);
        IQueryable<TEntity> GetAll();
        Task<ICollection<TEntity>> GetAllAsync();
        Task<ICollection<TEntity>> GetAllWithRelateAsync(Expression<Func<TEntity, bool>> match = null);
        Task<ICollection<TEntity>> GetAllWithIncludeAsync
            (List<Expression<Func<TEntity, object>>> relates);
        TEntity Find(Expression<Func<TEntity, bool>> match);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match);
        Task<ICollection<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match);
        Task<ICollection<TEntity>> FindAllWithIncludeAsync
            (Expression<Func<TEntity, bool>> match, List<Expression<Func<TEntity, object>>> relates);

        ICollection<TEntity> FindAllWithLazyLoad
            (Expression<Func<TEntity, bool>> match,
            List<Expression<Func<TEntity, object>>> relates,
            int Skip, int Row,
            Expression<Func<TEntity, string>> order = null,
            Expression<Func<TEntity, string>> orderDesc = null);

        Task<ICollection<TEntity>> FindAllWithLazyLoadAsync
            (Expression<Func<TEntity, bool>> match,
            List<Expression<Func<TEntity, object>>> relates,
            int Skip, int Row,
            Expression<Func<TEntity, string>> order = null,
            Expression<Func<TEntity, string>> orderDesc = null);
        TEntity Add(TEntity nTEntity);
        Task<TEntity> AddAsync(TEntity nTEntity);
        Task<IEnumerable<TEntity>> AddAllAsync(IEnumerable<TEntity> nTEntityList);
        Task<TEntity> UpdateAsync(TEntity updated, int key);
        Task<TEntity> UpdateAsync(TEntity updated, string key);
        TEntity Update(TEntity updated, string key);
        TEntity Update(TEntity updated, int key);
        void Delete(int key);
        Task<int> DeleteAsync(string TEntityId);
        Task<int> DeleteAsync(int key);
        Task<int> CountAsync();
        int CountWithMatch(Expression<Func<TEntity, bool>> match);
    }
}
