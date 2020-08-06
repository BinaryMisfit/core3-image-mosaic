namespace ImageMosaic
{
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class SourceImageFile
    {
        public IImageFile imageFile;

        public SourceImageFile(IImageFile imageFile)
        {
            this.imageFile = imageFile ?? throw new FileNotFoundException();
        }

        public Dictionary<string, Image> SplitImages { get; private set; }

        public void SplitImage(int columns, int rows, DirectoryInfo workingDirectory = null)
        {
            if (columns == 0 || rows == 0)
            {
                throw new MissingFieldException();
            }

            Image image;
            try
            {
                image = imageFile.Load();
            }
            catch (UnknownImageFormatException)
            {
                return;
            }

            int blockWidth = image.Width / columns;
            int blockHeight = image.Height / rows;
            if (blockWidth == 0 || blockHeight == 0)
            {
                throw new InvalidDataException();
            }

            bool saveFiles = workingDirectory != null && workingDirectory.Exists;
            SplitImages = new Dictionary<string, Image>();
            for (int x = 0; x < image.Width; x += blockWidth)
            {
                for (int y = 0; y < image.Height; y += blockHeight)
                {
                    Rectangle rectangle = new Rectangle(x, y, blockWidth, blockHeight);
                    string key = $"{x}x{y}";
                    Image splitImage = image.Clone(img => img.Crop(rectangle));
                    SplitImages.Add(key, splitImage);
                    if (saveFiles)
                    {
                        string saveFile = $"{workingDirectory.FullName}{Path.GetFileNameWithoutExtension(imageFile.File.FullName)}_{x}_{y}{imageFile.File.Extension}";
                        splitImage.Save(saveFile);
                    }
                }
            }
        }
    }
}