namespace MpsedcPortal.Services;

public interface IOtpService
{
    Task<string> GenerateOtpAsync(string identifier);
    Task<bool> VerifyOtpAsync(string identifier, string otp);
}

public class MockOtpService : IOtpService
{
    private readonly ILogger<MockOtpService> _logger;
    private readonly Dictionary<string, (string Otp, DateTime Expiry)> _otpStore = new();

    private const string MockOtp = "123456";
    private const int OtpExpiryMinutes = 5;

    public MockOtpService(ILogger<MockOtpService> logger)
    {
        _logger = logger;
    }

    public Task<string> GenerateOtpAsync(string identifier)
    {
        _otpStore[identifier] = (MockOtp, DateTime.UtcNow.AddMinutes(OtpExpiryMinutes));
        _logger.LogInformation("OTP generated for {Identifier}: {Otp} (MOCK - always 123456)", identifier, MockOtp);
        return Task.FromResult(MockOtp);
    }

    public Task<bool> VerifyOtpAsync(string identifier, string otp)
    {
        if (!_otpStore.TryGetValue(identifier, out var stored))
            return Task.FromResult(false);

        if (DateTime.UtcNow > stored.Expiry)
        {
            _otpStore.Remove(identifier);
            return Task.FromResult(false);
        }

        var isValid = stored.Otp == otp;
        if (isValid)
            _otpStore.Remove(identifier);

        return Task.FromResult(isValid);
    }
}
