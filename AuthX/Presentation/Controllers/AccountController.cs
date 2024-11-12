using AuthX.Application.IServices;
using AuthX.Presentation.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthX.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAccountService accountService, ILogger<AccountController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginUser)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var loginResponse = await _accountService.Login(loginUser.UserName, loginUser.Password);
        return Ok(loginResponse);
    }

    [Authorize]
    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        var token = "";
        var hasLoggedout = await _accountService.Logout(token);

        if (hasLoggedout)
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }
}