using AuthX.Domain.Models;

namespace AuthX.Domain.IRepositories;

public interface ISessionRepository
{
    Task SetSessionDataAsync(string token, Session sessionData);
    Task<Session> GetSessionDataAsync(string token);
}