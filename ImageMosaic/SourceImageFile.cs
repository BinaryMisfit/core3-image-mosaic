namespace ImageMosaic
{
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Formats;
    using SixLabors.ImageSharp.Processing;
    using System;
    using System.Collections.Generic;
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

        public Dictionary<string, Image> SplitImages { get; private set; }

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

        public void SplitImage(int columns, int rows)
        {
            if (columns == 0 || rows == 0)
            {
                throw new MissingFieldException();
            }

            IImageFormat format;
            Image image = LoadImage(out format);
            if (image == null)
            {
                throw new FileNotFoundException();
            }

            int blockWidth = image.Width / columns;
            int blockHeight = image.Height / rows;
            if (blockWidth == 0 || blockHeight == 0)
            {
                throw new InvalidDataException();
            }

            SplitImages = new Dictionary<string, Image>();
            for (int x = 0; x < image.Width; x += blockWidth)
            {
                for (int y = 0; y < image.Height; y += blockHeight)
                {
                    Rectangle rectangle = new Rectangle(x, y, blockWidth, blockHeight);
                    string key = $"{x}x{y}";
                    Image splitImage = image.Clone(img => img.Crop(rectangle));
                    SplitImages.Add(key, splitImage);
                }
            }
        }

        private Image LoadImage(out IImageFormat format)
        {
            try
            {
                return Image.Load(file.OpenRead(), out format);
            }
            catch (UnknownImageFormatException)
            {
                format = null;
                return null;
            }
        }
    }
}