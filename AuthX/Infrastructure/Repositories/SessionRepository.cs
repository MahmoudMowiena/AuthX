using System.Text.Json;
using AuthX.Domain.IRepositories;
using AuthX.Domain.Models;
using StackExchange.Redis;

namespace AuthX.Infrastructure.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly IConnectionMultiplexer _redisConnection;
    private readonly IDatabase _database;
    private readonly TimeSpan _expirationTime;
    private readonly IConfiguration _configuration;

    public SessionRepository(IConnectionMultiplexer redisConnection, IConfiguration configuration)
    {
        _redisConnection = redisConnection;
        _database = _redisConnection.GetDatabase();
        _configuration = configuration;
        var expirationTimeAsInt = _configuration.GetValue<int>("Session:ExpirationTime");
        _expirationTime = TimeSpan.FromMinutes(expirationTimeAsInt);
    }

    public async Task SetSessionDataAsync(string token, Session session)
    {
        var serializedSession = JsonSerializer.Serialize(session);
        await _database.StringSetAsync(token, serializedSession, _expirationTime);
    }

    public async Task<Session> GetSessionDataAsync(string token)
    {
        var serializedSession = await _database.StringGetAsync(token);

        if (string.IsNullOrEmpty(serializedSession))
        {
            return null;
        }

        var session = JsonSerializer.Deserialize<Session>(serializedSession);
        return session;
    }
}