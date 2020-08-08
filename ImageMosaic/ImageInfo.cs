using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using System.IO;

namespace ImageMosaic
{
    public class ImageInfo
    {
        public ImageInfo(FileInfo file)
        {
            if (!file.Exists)
            {
                throw new FileNotFoundException();
            }

            File = file;
            if (!ValidateImage(file.FullName))
            {
                throw new UnknownImageFormatException($"{file.Name} not an image");
            }

            Id = file.FullName;
        }

        public ImageInfo(string id, IImageFormat format, Image image)
        {
            if (image == null)
            {
                throw new UnknownImageFormatException("Missing image");
            }

            Id = id;
            Format = format;
            Width = image.Width;
            Height = image.Height;
            ColorTable = new ImageCalculate(image).ColorTable;
        }

        public ImageColorTable ColorTable { get; private set; }
        public FileInfo File { get; }
        public IImageFormat Format { get; private set; }
        public int Height { get; private set; }
        public string Id { get; }
        public int Width { get; private set; }

        private bool ValidateImage(string fileName)
        {
            try
            {
                using (Image image = Image.Load(fileName, out IImageFormat format))
                {
                    Width = image.Width;
                    Height = image.Height;
                    Format = format;
                    ColorTable = new ImageCalculate(image).ColorTable;
                    return true;
                }
            }
            catch (UnknownImageFormatException)
            {
                return false;
            }
        }
    }
}