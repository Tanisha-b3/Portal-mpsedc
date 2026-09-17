using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace MpsedcPortal.Services;

public class ImageService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ImageService> _logger;

    private static readonly string[] AllowedTypes = { "image/jpeg", "image/png", "image/webp" };
    private const long MaxFileSize = 5 * 1024 * 1024;
    private const int MaxDimension = 1200;

    public ImageService(IWebHostEnvironment env, ILogger<ImageService> logger)
    {
        _env = env;
        _logger = logger;
    }

    public async Task<string> OptimizeProfileImage(IFormFile file)
    {
        if (!AllowedTypes.Contains(file.ContentType.ToLower()))
            throw new InvalidOperationException("Invalid image format. Allowed: JPG, PNG, WebP.");

        if (file.Length > MaxFileSize)
            throw new InvalidOperationException("Image must be less than 5 MB.");

        var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "profiles");
        Directory.CreateDirectory(uploadDir);

        using var stream = file.OpenReadStream();
        using var image = await Image.LoadAsync(stream);

        if (image.Width > MaxDimension || image.Height > MaxDimension)
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(MaxDimension, MaxDimension)
            }));
        }

        var fileName = $"{Guid.NewGuid()}.webp";
        var path = Path.Combine(uploadDir, fileName);

        var encoder = new WebpEncoder { Quality = 90 };
        await image.SaveAsync(path, encoder);

        _logger.LogInformation("Profile image optimized: {OriginalSize} -> {NewPath}", file.Length, fileName);

        return fileName;
    }

    public void DeleteImage(string fileName)
    {
        var path = Path.Combine(_env.WebRootPath, "uploads", "profiles", fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
