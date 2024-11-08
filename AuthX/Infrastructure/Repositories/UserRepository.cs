using AuthX.Domain.IRepositories;
using AuthX.Domain.Models;
using AuthX.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace AuthX.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AuthXDbContext context) : base(context) {}

    public async Task<User> FindByUsername(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.UserName == username);
    }
}