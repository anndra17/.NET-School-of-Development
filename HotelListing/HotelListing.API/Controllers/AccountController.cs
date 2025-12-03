using HotelListing.API.Contracts;
using HotelListing.API.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly IAuthManager _authManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAuthManager authManager, ILogger<AccountController> logger)
    {
        _authManager = authManager;
        _logger = logger;
    }

    // api/Account/login
    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
    {
        _logger.LogInformation($"Login Attempt for {loginDto.Email}");
        try
        {
            var authResponse = await _authManager.Login(loginDto);

            if (authResponse is null)
            {
                return Unauthorized();
            }

            return Ok(authResponse);
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, $"Something Went Wrong in the {nameof(Login)} - User Login attemtp for {loginDto.Email}\"");
            return Problem($"Something Went Wrong in the {nameof(Login)}", statusCode: 500);
        }
    }

    // api/Account/register
    [HttpPost]
    [Route("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Register([FromBody] ApiUserDto apiUserDto)
    {
        _logger.LogInformation($"Registration Attempt for {apiUserDto.Email}");

        try
        {
            var errors = await _authManager.Register(apiUserDto);

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            return Ok();
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, $"Something Went Wrong in the {nameof(Register)} - User Registration attemtp for {apiUserDto.Email}");
            return Problem($"Something Went Wrong in the {nameof(Register)}", statusCode: 500);
        }
    }

    // api/Account/refreshtoken
    [HttpPost]
    [Route("refreshtoken")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> RefreshToken([FromBody] AuthResponseDto request)
    {
        _logger.LogInformation($"RefreshToken Attempt for user with ID {request.UserId}");

        try
        {
            var authResponse = await _authManager.VerifyRefreshToken(request);

            if (authResponse is null)
            {
                return Unauthorized();
            }

            return Ok(authResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something Went Wrong in the {nameof(Register)} - Refresh Token attemtp for User with ID {request.UserId}");
            return Problem($"Something Went Wrong in the {nameof(RefreshToken)}", statusCode: 500);
        }
    }
}
