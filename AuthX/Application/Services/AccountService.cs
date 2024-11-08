using AuthX.Application.IServices;
using AuthX.Domain.IRepositories;
using AuthX.Domain.Models;
using AuthX.Presentation.DTOs;
using Microsoft.AspNetCore.Identity;

namespace AuthX.Application.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly ISessionRepository _sessionRepository;

    public AccountService(UserManager<User> userManager, RoleManager<Role> roleManager, ISessionRepository sessionRepository)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _sessionRepository = sessionRepository;
    }

    public async Task<LoginResponse> Login(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);

        if (!isPasswordCorrect)
            throw new UnauthorizedAccessException("Invalid credentials");

        var token = GenerateJwtToken(user);
        //_sessionRepository.SetSessionDataAsync(token);
        return new LoginResponse { Token = token };
    }

    public Task Logout(string username)
    {
        throw new NotImplementedException();
    }

    private string GenerateJwtToken(User user)
    {
        return "";
    }
}