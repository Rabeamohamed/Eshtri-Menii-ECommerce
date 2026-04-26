using ECom.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace ECom.Infrastructure.Service
{
    public class ImageManagementService : IImageManagementService
    {
        private readonly IFileProvider fileProvider;
        public ImageManagementService(IFileProvider fileProvider)
        {
            this.fileProvider = fileProvider;
        }
        public async Task<List<string>> AddImageAsync(IFormFileCollection files, string src)
        {
            List<string> SaveImageSrc = new List<string>(); // List to Save Image Src,List Because the product may have more than one image
            var ImageDirectory = Path.Combine("wwwroot","Images", src);

            if (!Directory.Exists(ImageDirectory)) // Check if Directory Exists or not 
            {
                Directory.CreateDirectory(ImageDirectory); // Create Directory if not exists
            }

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Get Image Name
                    var ImageName = file.FileName;
                    
                    var ImageSrc = $"Images/{src}/{ImageName}"; // Create Image Src
                    var root = Path.Combine(ImageDirectory, ImageName); // Create Root Path
                    using (FileStream stream = new FileStream(root, FileMode.Create)) // Save Image to folder
                    {
                        await file.CopyToAsync(stream);
                    }
                    
                    SaveImageSrc.Add(ImageSrc);
                }
            }
            return SaveImageSrc;

        }

        public void DeleteImageAsync(string src)
        {
            var info= fileProvider.GetFileInfo(src);

            var root = info.PhysicalPath;
            File.Delete(root);
        }
    }
}
