using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace DataShareData.Repository;
using DataShareCore.Models;


public abstract class RepositoryBase<TModel> : IRepositoryBase<TModel> where TModel : ModelBase
    {
        public IQueryable<TModel> Query => _context.Set<TModel>().AsQueryable();
        private readonly DataContext _context;
        public RepositoryBase(DataContext dbContext)
        {
            _context = dbContext;
        }
        public TModel Create(TModel entity)
        {
            return _context.Add(entity).Entity;
        }
        public virtual async Task<TModel> CreateAsync(TModel entity)
        {
            var result = await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }
        public TModel GetById(int id)
        {
            return _context.Set<TModel>().Find(id);
        }

        public virtual async Task<TModel> GetByIdAsync(int id)
        {
            return await _context.Set<TModel>().FindAsync(id);
        }

        public TModel GetById(Expression<Func<TModel, bool>> predicate)
        {
            return _context.Set<TModel>().Where(predicate).FirstOrDefault();
        }

        public async Task<TModel> GetByIdAsync(Expression<Func<TModel, bool>> predicate)
        {
            return await _context.Set<TModel>().Where(predicate).FirstOrDefaultAsync();
        }

        public IEnumerable<TModel> GetAll()
        {
            return _context.Set<TModel>().ToList();
        }

        public virtual async Task<IEnumerable<TModel>> GetAllAsync()
        {
            return await _context.Set<TModel>().ToListAsync();
        }
        public TModel Update(TModel entity)
        {
            return _context.Update(entity).Entity;
        }

        public virtual async Task<TModel> UpdateAsync(TModel entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public TModel Delete(int id)
        {
            var entity = GetById(id);
            if (entity is null) return default;
            _context.Remove(entity);
            _context.SaveChanges(); // This line commits the deletion to the database.
            return entity;
        }

        public virtual async Task<TModel> DeleteAsync(int id)
        {
            return await Task.Run(() =>
            {
                var entity = GetById(id);
                if (entity is null) return default;
                return _context.Remove(entity).Entity;
            });
        }
    }