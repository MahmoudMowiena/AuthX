using AuthX.Domain.Models;

namespace AuthX.Domain.IRepositories;

public interface ISessionRepository
{
    Task SetSessionDataAsync(Session session);
    Task<Session> GetSessionDataAsync(string token);
    Task<bool> DeleteSessionAsync(string token);
}