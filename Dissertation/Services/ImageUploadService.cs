using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Dissertation.Services
{
    public interface IImageUploadService
    {
        Task GenerateThumbnail(string originalFilePath, string thumbnailPath);
        Task<string?> UploadImage(IFormFile imageFile, string imagePath, string thumbnailPath);
        void DeleteImage(string filePath, string thumbnailPath);
    }

    public class ImageUploadService : IImageUploadService
    {
        private readonly string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

        public async Task GenerateThumbnail(string filePath, string thumbnailPath)
        {
            int currentWidth;
            int currentHeight;
            int maxWidth = 240;
            int maxHeight = 135;

            using (var image = await Image.LoadAsync(filePath))
            {
                currentHeight = image.Height;
                currentWidth = image.Width;
                // Checks if the image is larger than the maximum dimensions
                if (currentWidth > maxWidth || currentHeight > maxHeight)
                {
                    int newWidth;
                    int newHeight;
                    // Checks if the image is landscape or portrait
                    if (currentWidth > currentHeight)
                    {
                        // Resizes the image to fit within the maximum dimensions while maintaining the aspect ratio
                        newWidth = maxWidth;
                        newHeight = (int)Math.Round((float)(maxWidth / currentWidth * currentHeight));
                    }
                    else
                    {
                        newHeight = maxHeight;
                        newWidth = (int)Math.Round((float)(maxHeight / currentHeight * currentWidth));
                    }
                    // Resizes the image
                    image.Mutate(x => x.Resize(newWidth, newHeight));
                }
                // Saves the image as a WebP file with the size modifications if applied
                await image.SaveAsWebpAsync(thumbnailPath);
            }
        }

        public async Task<string?> UploadImage(IFormFile imageFile, string filePath, string thumbnailPath)
        {
            string fileExtension = Path.GetExtension(imageFile.FileName);
            if (!allowedExtensions.Contains(fileExtension))
            {
                return "Invalid file type. Only JPG, JPEG, PNG, and WebP files are allowed.";
            }

            string fileDirectory = Path.GetDirectoryName(filePath);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);

            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            await GenerateThumbnail(filePath, thumbnailPath);

            return null;
        }

        public void DeleteImage(string imagePath, string thumbnailPath)
        {
            string fullImagePath = "wwwroot" + imagePath;
            string fullThumbnailPath = "wwwroot" + thumbnailPath;
            if (File.Exists(fullImagePath))
            {
                File.Delete(fullImagePath);
            }
            if (File.Exists(fullThumbnailPath))
            {
                File.Delete(fullThumbnailPath);
            }
        }
    }
}
