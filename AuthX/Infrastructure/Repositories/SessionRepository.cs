using System.Text.Json;
using AuthX.Domain.IRepositories;
using AuthX.Domain.Models;
using Microsoft.AspNetCore.SignalR;
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

    public async Task SetSessionDataAsync(Session session)
    {
        var serializedSession = Serialize(session);
        await _database.HashSetAsync(session.Token, serializedSession);
        await _database.KeyExpireAsync(session.Token, _expirationTime);
    }

    public async Task<Session> GetSessionDataAsync(string token)
    {
        var serializedSession = await _database.HashGetAllAsync(token);

        if (serializedSession.Length == 0)
        {
            return null;
        }

        var session = Deserialize(token, serializedSession);
        return session;
    }

    public async Task<bool> DeleteSessionAsync(string token)
    {
        return await _database.KeyDeleteAsync(token);
    }

    private HashEntry[] Serialize(Session session)
    {
        var createdDateMilliseconds = new DateTimeOffset(session.CreatedDate).ToUnixTimeMilliseconds();
        var expirationDateMilliseconds = new DateTimeOffset(session.ExpirationDate).ToUnixTimeMilliseconds();

        return
        [
            new HashEntry(nameof(session.UserID), session.UserID),
            new HashEntry(nameof(session.CreatedDate), createdDateMilliseconds),
            new HashEntry(nameof(session.ExpirationDate), expirationDateMilliseconds),
        ];
    }

    private Session Deserialize(string token, HashEntry[] serializedSession)
    {
        var session = new Session
        {
            Token = token
        };

        foreach (var entry in serializedSession)
        {
            switch (entry.Name)
            {
                case nameof(Session.UserID):
                    session.UserID = (string)entry.Value;
                    break;
                case nameof(Session.CreatedDate):
                    var createdDateMilliseconds = (long)entry.Value;
                    session.CreatedDate = DateTimeOffset.FromUnixTimeMilliseconds(createdDateMilliseconds).UtcDateTime;
                    break;
                case nameof(Session.ExpirationDate):
                    var expirationDateMilliseconds = (long)entry.Value;
                    session.ExpirationDate = DateTimeOffset.FromUnixTimeMilliseconds(expirationDateMilliseconds).UtcDateTime;
                    break;
            }
        }

        return session;
    }
}