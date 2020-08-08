namespace ImageMosaic
{
    using SixLabors.ImageSharp;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class FileParser
    {
        public static List<ImageInfo> ParseDirectory(DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                throw new DirectoryNotFoundException();
            }

            List<ImageInfo> images = new List<ImageInfo>();
            directory.GetFiles("*", SearchOption.AllDirectories).ToList().ForEach(file =>
            {
                try
                {
                    ImageInfo image = new ImageInfo(file);
                    images.Add(image);
                }
                catch (UnknownImageFormatException)
                {
                    file.Delete();
                }
            });

            if (images.Count <= 1)
            {
                throw new FileNotFoundException();
            }

            return images;
        }

        public static ImageInfo ParseFile(FileInfo file)
        {
            ImageInfo image = new ImageInfo(file);
            return image;
        }
    }
}