using Blog_MVC.Services.Interfaces;

namespace Blog_MVC.Services
{
    public class ImageService : IImageService
    {
        private readonly string _defaultBlogPostImageSrc = "/img/DefaultWritingPic.png";
        private readonly string _defaultCategoryImageSrc = "/img/DefaultBlog2.png";
        private readonly string _defaultUserImageSrc = "/img/DefaultContactImage.png";

        public string ConvertByteArrayToFile(byte[] fileData, string extension, int defaultImage)
        {
            if (fileData == null || fileData.Length == 0)
            {
                switch (defaultImage)
                {
                    case 1: return _defaultUserImageSrc;
                    case 2: return _defaultBlogPostImageSrc;
                    case 3: return _defaultCategoryImageSrc;
                }
            }

            // try-catch statement will allow application to run despite exceptions/errors
            try
            {
                // converts the byte array to string and outputs as variable imageBase64Data
                string imageBase64Data = Convert.ToBase64String(fileData!);
                // formats the data into a string that can be read by HTML img tag
                string imageSrcString = string.Format($"data:{extension};base64,{imageBase64Data}");

                return imageSrcString;

            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                // the using statement will clean up after itself by immediately reallocating the used memory
                // MemoryStream reads, buffers, and creates a cache memory of incoming data before feeding it to the PC for use
                using MemoryStream memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                byte[] byteFile = memoryStream.ToArray();
                memoryStream.Close();
                return byteFile;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
