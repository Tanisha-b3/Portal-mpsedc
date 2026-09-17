using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MpsedcPortal.DTOs;
using MpsedcPortal.Models;
using MpsedcPortal.Services;

namespace MpsedcPortal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ImageService _imageService;
    private readonly IOtpService _otpService;
    private readonly ICaptchaService _captchaService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        ImageService imageService,
        IOtpService otpService,
        ICaptchaService captchaService,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _imageService = imageService;
        _otpService = otpService;
        _captchaService = captchaService;
        _logger = logger;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromForm] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new RegisterResponse
            {
                Success = false,
                Message = "Validation failed",
                Errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList()
            });

        if (!_captchaService.Validate(request.Captcha))
        {
            return BadRequest(new RegisterResponse
            {
                Success = false,
                Message = "CAPTCHA verification failed"
            });
        }

        var existingEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingEmail != null)
        {
            return BadRequest(new RegisterResponse
            {
                Success = false,
                Message = "Email is already registered"
            });
        }

        var existingUsername = await _userManager.FindByNameAsync(request.Username);
        if (existingUsername != null)
        {
            return BadRequest(new RegisterResponse
            {
                Success = false,
                Message = "Username is already taken"
            });
        }

        string? imageName = null;
        if (request.ProfileImage != null)
        {
            try
            {
                imageName = await _imageService.OptimizeProfileImage(request.ProfileImage);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new RegisterResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        var user = new ApplicationUser
        {
            FullName = request.FullName,
            Email = request.Email,
            UserName = request.Username,
            PhoneNumber = request.Mobile,
            ProfileImageName = imageName,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            if (imageName != null)
                _imageService.DeleteImage(imageName);

            return BadRequest(new RegisterResponse
            {
                Success = false,
                Message = "Registration failed",
                Errors = result.Errors.Select(e => e.Description).ToList()
            });
        }

        _logger.LogInformation("User registered: {Username} ({Email})", request.Username, request.Email);

        await _otpService.GenerateOtpAsync(request.Email);

        return Ok(new RegisterResponse
        {
            Success = true,
            Message = "Registration successful. Please verify your email with the OTP sent.",
            ProfileImageName = imageName
        });
    }

    [HttpPost("send-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
    {
        if (string.IsNullOrEmpty(request.Identifier))
            return BadRequest(new { message = "Email or mobile is required" });

        await _otpService.GenerateOtpAsync(request.Identifier);

        return Ok(new { message = "OTP sent successfully (Mock: use 123456)" });
    }

    [HttpPost("verify-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyRequest request)
    {
        var isValid = await _otpService.VerifyOtpAsync(request.Identifier, request.Otp);

        if (!isValid)
            return BadRequest(new { message = "Invalid or expired OTP" });

        return Ok(new { message = "OTP verified successfully" });
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();

        return Ok(new
        {
            user.FullName,
            user.Email,
            user.UserName,
            user.PhoneNumber,
            user.ProfileImageName,
            user.IsEmailVerified,
            user.IsMobileVerified,
            user.CreatedAt
        });
    }
}
