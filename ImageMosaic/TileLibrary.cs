namespace ImageMosaic
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class TileLibrary
    {
        public TileLibrary(DirectoryInfo directory)
        {
            Directory = directory ?? throw new DirectoryNotFoundException();
            if (!directory.Exists)
            {
                throw new DirectoryNotFoundException();
            }
        }

        public List<TileImage> Images { get; private set; }

        internal DirectoryInfo Directory { get; }

        public bool HasImages()
        {
            List<FileInfo> files = Directory.GetFiles("*", SearchOption.AllDirectories).ToList();
            if (files.Count == 0)
            {
                return false;
            }

            List<TileImage> images = new List<TileImage>();
            foreach (FileInfo file in files)
            {
                ImageFile imageFile = new ImageFile(file);
                if (imageFile.IsImageFile)
                {
                    TileImage tileImage = new TileImage(imageFile.File.FullName, imageFile.Load());
                    images.Add(tileImage);
                }
            }

            Images = images;
            return Images.Count > 1;
        }

        public void Remove(List<TileImage> images)
        {
            Images = Images.Except(images).ToList();
        }
    }
}