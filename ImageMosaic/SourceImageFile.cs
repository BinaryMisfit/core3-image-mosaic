namespace ImageMosaic
{
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Formats;
    using System.IO;

    internal class SourceImageFile
    {
        private readonly FileInfo file;

        public SourceImageFile(FileInfo file)
        {
            this.file = file;
            if (file == null)
            {
                throw new FileNotFoundException();
            }
        }

        public string FileName => file.FullName;

        public bool IsImageFile()
        {
            try
            {
                using (Image image = Image.Load(file.OpenRead(), out IImageFormat format))
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
}