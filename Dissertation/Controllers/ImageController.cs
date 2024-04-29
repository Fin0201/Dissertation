using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Dissertation.Controllers
{
    public class ImageService : Controller
    {
        private readonly string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

        public async void GenerateThumbnail(IFormFile imageFile)
        {
            var imageBytes = new byte[imageFile.Length];
            await imageFile.OpenReadStream().ReadAsync(imageBytes);
            using var inStream = new MemoryStream(imageBytes);
            using var myImage = await Image.LoadAsync(inStream);

            int currentWidth;
            int currentHeight;
            int maxWidth = 180;
            int maxHeight = 135;
            using (var stream = imageFile.OpenReadStream())
            {
                using (var image = await Image.LoadAsync(stream))
                {
                    currentHeight = image.Height;
                    currentWidth = image.Width;
                }
            }

            // Checks if the image is larger than the maximum dimensions
            if (currentWidth > maxWidth || currentHeight > maxHeight)
            {
                int newWidth;
                int newHeight;

                // Resizes the image to fit within the maximum dimensions while maintaining the aspect ratio
                if (currentWidth > currentHeight)
                {
                    newWidth = maxWidth;
                    newHeight = (int)Math.Round((float)(maxWidth / currentWidth * currentHeight));
                }
                else
                {
                    newHeight = maxHeight;
                    newWidth = (int)Math.Round((float)(maxHeight / currentHeight * currentWidth));
                }
                myImage.Mutate(x => x.Resize(newWidth, newHeight));
            }

            using var outStream = new MemoryStream();
            await myImage.SaveAsync(outStream, new WebpEncoder());
            return;
        }

        public string? UploadImage(IFormFile imageFile)
        {
            string fileExtension = Path.GetExtension(imageFile.FileName);
            if (!allowedExtensions.Contains(fileExtension))
            {
                return "Invalid file type. Only JPG, JPEG, PNG, and WebP files are allowed.";
            }

            string fileName = Guid.NewGuid().ToString() + fileExtension;
            string containingFolder = "wwwroot/images/user-uploads";
            string imagePath = Path.Combine(containingFolder, fileName);

            if (!Directory.Exists(containingFolder))
            {
                Directory.CreateDirectory(containingFolder);
            }
            return null;
        }
    }
}
