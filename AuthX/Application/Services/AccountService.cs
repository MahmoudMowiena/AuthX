using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthX.Application.IServices;
using AuthX.Domain.IRepositories;
using AuthX.Domain.Models;
using AuthX.Presentation.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AuthX.Application.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly ISessionRepository _sessionRepository;
    private readonly IConfiguration _configuration;

    public AccountService(UserManager<User> userManager, RoleManager<Role> roleManager, ISessionRepository sessionRepository, IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _sessionRepository = sessionRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponse> Login(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username) ?? throw new UnauthorizedAccessException("Invalid credentials");
        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);

        if (!isPasswordCorrect)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var token = await GenerateJwtToken(user);

        var session = new Session
        {
            Token = token,
            UserID = user.Id,
            CreatedDate = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow, // set that correctly
        };

        await _sessionRepository.SetSessionDataAsync(session);

        var returnedUser = new ReturnedUser
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        return new LoginResponse { Token = token, User = returnedUser };
    }

    public async Task<bool> Logout(string token)
    {
        return await _sessionRepository.DeleteSessionAsync(token);
    }

    private async Task<string> GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        //role
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
            claims.Add(new Claim("role", role));
        }

        SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
        SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        JwtSecurityToken jwtSecurityToken =
            new(
                issuer: _configuration["JWT:ValidIssuer"],  // url web api 
                audience: _configuration["JWT:ValidAudiance"], // url consumer angular
                claims: claims,
                expires: DateTime.UtcNow.AddHours(5),
                signingCredentials: signingCredentials
                );

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return jwtToken;
    }
}