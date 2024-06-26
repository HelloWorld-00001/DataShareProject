using System.Linq.Expressions;

namespace DataShareData.Repository;
using DataShareCore.Models;
public interface IRepositoryBase<TModel> where TModel : ModelBase
{
    IQueryable<TModel> Query { get; }

    TModel GetById(int id);
    Task<TModel> GetByIdAsync(int id);

    TModel GetById(Expression<Func<TModel, bool>> predicate);
    Task<TModel> GetByIdAsync(Expression<Func<TModel, bool>> predicate);

    IEnumerable<TModel> GetAll();
    Task<IEnumerable<TModel>> GetAllAsync();


    TModel Create(TModel entity);
    Task<TModel> CreateAsync(TModel entity);

    TModel Update(TModel entity);
    Task<TModel> UpdateAsync(TModel entity);

    TModel Delete(int id);
}