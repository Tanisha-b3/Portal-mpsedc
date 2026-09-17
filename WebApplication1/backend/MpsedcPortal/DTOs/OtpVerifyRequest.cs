namespace MpsedcPortal.DTOs;

public class OtpVerifyRequest
{
    public string Identifier { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
}

public class SendOtpRequest
{
    public string Identifier { get; set; } = string.Empty;
}
