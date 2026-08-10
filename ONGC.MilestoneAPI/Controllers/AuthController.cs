using Microsoft.AspNetCore.Mvc;
using ONGC.MilestoneAPI.Helpers;
using ONGC.MilestoneAPI.Models.DTOs;
using ONGC.MilestoneAPI.Models.Entities;
using ONGC.MilestoneAPI.Models.Enums;
using ONGC.MilestoneAPI.Repositories.Interfaces;

namespace ONGC.MilestoneAPI.Controllers;

/// <summary>
/// Controller for authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserRepository userRepository,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <returns>Success message</returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Failed",
                Detail = "Please check the provided data.",
                Instance = HttpContext.Request.Path
            });
        }

        // Check if email already exists
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            _logger.LogWarning("Registration attempt with existing email: {Email}", request.Email);
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Email Already Exists",
                Detail = "A user with this email address already exists.",
                Instance = HttpContext.Request.Path
            });
        }

        // Parse role or default to User
        UserRole role = UserRole.User;
        if (!string.IsNullOrWhiteSpace(request.Role) &&
            !Enum.TryParse<UserRole>(request.Role, true, out role))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid Role",
                Detail = "The provided role is not valid. Valid roles: Admin, User, Viewer",
                Instance = HttpContext.Request.Path
            });
        }

        // Hash password using BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = role
        };

        await _userRepository.AddAsync(user);

        _logger.LogInformation("User registered successfully: {Email}, Role: {Role}", user.Email, user.Role);

        return CreatedAtAction(
            nameof(Register),
            new { id = user.Id },
            new { message = "User registered successfully", userId = user.Id, email = user.Email, role = user.Role.ToString() });
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token and user details</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Failed",
                Detail = "Please check the provided data.",
                Instance = HttpContext.Request.Path
            });
        }

        // Get user by email
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            _logger.LogWarning("Login attempt with non-existent email: {Email}", request.Email);
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Invalid Credentials",
                Detail = "Email or password is incorrect.",
                Instance = HttpContext.Request.Path
            });
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Failed login attempt for user: {Email}", request.Email);
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Invalid Credentials",
                Detail = "Email or password is incorrect.",
                Instance = HttpContext.Request.Path
            });
        }

        // Generate JWT token
        var token = JwtTokenHelper.GenerateToken(user, _configuration);
        var expiresAt = JwtTokenHelper.GetTokenExpiration(_configuration);

        _logger.LogInformation("User logged in successfully: {Email}", user.Email);

        var response = new LoginResponse
        {
            Token = token,
            Email = user.Email,
            Role = user.Role.ToString(),
            ExpiresAt = expiresAt
        };

        return Ok(response);
    }
}
