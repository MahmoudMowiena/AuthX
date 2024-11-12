using AuthX.Domain.Models;

namespace AuthX.Domain.IRepositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User> FindByUsername(string username);
}