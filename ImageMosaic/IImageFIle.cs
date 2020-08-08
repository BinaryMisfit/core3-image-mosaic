namespace ImageMosaic
{
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Formats;
    using System.IO;

    public interface IImageFile
    {
        public FileInfo File { get; }

        public IImageFormat Format { get; }

        public bool IsImageFile { get; }

        public Image Load();
    }
}