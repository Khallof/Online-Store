namespace Store.API.Services
{
    public interface IImageUploadService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder = "products");
        Task<bool> DeleteImageAsync(string imageUrl);
        bool IsValidImage(IFormFile file);
        string GetValidationError(IFormFile file);
    }

    public class ImageUploadService : IImageUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ImageUploadService> _logger;

        private static readonly HashSet<string> AllowedExtensions = new()
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp"
        };

        private static readonly HashSet<string> AllowedMimeTypes = new()
        {
            "image/jpeg", "image/png", "image/gif", "image/webp"
        };

        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB

        public ImageUploadService(
            IWebHostEnvironment environment,
            ILogger<ImageUploadService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder = "products")
        {
            if (!IsValidImage(file))
                throw new InvalidOperationException(GetValidationError(file));

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", folder);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uniqueName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = $"/images/{folder}/{uniqueName}";
            _logger.LogInformation("Image uploaded: {ImageUrl}", imageUrl);
            return imageUrl;
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            try
            {
                var relativePath = imageUrl.TrimStart('/');
                var filePath = Path.Combine(_environment.WebRootPath, relativePath);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation("Image deleted: {ImageUrl}", imageUrl);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image: {ImageUrl}", imageUrl);
                return false;
            }
        }

        public bool IsValidImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return false;
            if (file.Length > MaxFileSizeBytes) return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension)) return false;
            if (!AllowedMimeTypes.Contains(file.ContentType.ToLowerInvariant())) return false;

            return true;
        }

        public string GetValidationError(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return "No file was provided.";
            if (file.Length > MaxFileSizeBytes)
                return $"File size exceeds the 5MB limit.";

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return $"File type '{extension}' not allowed. Use: JPG, JPEG, PNG, GIF, WEBP.";
            if (!AllowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
                return $"Invalid file content type '{file.ContentType}'.";

            return "Invalid image file.";
        }
    }
}