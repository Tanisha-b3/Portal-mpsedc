namespace MpsedcPortal.Services;

public interface ICaptchaService
{
    bool Validate(string? captchaResponse);
}

public class MockCaptchaService : ICaptchaService
{
    private readonly ILogger<MockCaptchaService> _logger;

    public MockCaptchaService(ILogger<MockCaptchaService> logger)
    {
        _logger = logger;
    }

    public bool Validate(string? captchaResponse)
    {
        if (string.IsNullOrEmpty(captchaResponse))
        {
            _logger.LogWarning("CAPTCHA validation failed: empty response");
            return false;
        }

        _logger.LogInformation("CAPTCHA validated (MOCK - always passes)");
        return true;
    }
}
