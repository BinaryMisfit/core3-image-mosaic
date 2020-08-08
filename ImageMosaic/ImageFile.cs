namespace ImageMosaic
{
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Formats;
    using System.IO;

    public class ImageFile : IImageFile
    {
        private IImageFormat format;

        public ImageFile(FileInfo file)
        {
            File = file ?? throw new FileNotFoundException();
        }

        public FileInfo File { get; }

        public IImageFormat Format => this.format;

        public bool IsImageFile
        {
            get
            {
                try
                {
                    using (Image image = Image.Load(File.OpenRead(), out IImageFormat Format))
                    {
                        return true;
                    }
                }
                catch (UnknownImageFormatException)
                {
                    return false;
                }
            }
        }

        public Image Load()
        {
            return Image.Load(File.OpenRead(), out format);
        }
    }
}