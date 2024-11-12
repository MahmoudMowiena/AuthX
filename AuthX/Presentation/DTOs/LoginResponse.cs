using AuthX.Domain.Models;

namespace AuthX.Presentation.DTOs;

public class LoginResponse
{
    public string Token { get; set; }
    public ReturnedUser User { get; set; }
}