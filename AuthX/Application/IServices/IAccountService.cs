using AuthX.Presentation.DTOs;

namespace AuthX.Application.IServices;

public interface IAccountService
{
    Task<LoginResponse> Login(string username, string password);
    Task Logout(string username);
}