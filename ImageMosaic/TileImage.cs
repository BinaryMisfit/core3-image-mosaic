namespace ImageMosaic
{
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public static class TileImage
    {
        public static Dictionary<string, Image> Tile(FileInfo imageFile, int columns, int rows)
        {
            if (columns == 0 || rows == 0)
            {
                throw new MissingFieldException();
            }

            using (Image image = Image.Load(imageFile.FullName))
            {
                int blockWidth = image.Width / columns;
                int blockHeight = image.Height / rows;
                if (blockWidth == 0 || blockHeight == 0)
                {
                    throw new InvalidDataException();
                }

                Dictionary<string, Image> splitImages = new Dictionary<string, Image>();
                for (int x = 0; x < image.Width; x += blockWidth)
                {
                    for (int y = 0; y < image.Height; y += blockHeight)
                    {
                        Rectangle rectangle = new Rectangle(x, y, blockWidth, blockHeight);
                        string key = $"{x}x{y}";
                        Image splitImage = image.Clone(img => img.Crop(rectangle));
                        splitImages.Add(key, splitImage);
                    }
                }

                return splitImages;
            }
        }
    }
}