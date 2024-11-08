namespace AuthX.Domain.IRepositories;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll();
    Task Add(T entity);
    void Update(T entity);
    void Remove (T entity);
}