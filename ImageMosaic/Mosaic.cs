using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections.Generic;
using System.Linq;

namespace ImageMosaic
{
    public static class Mosaic
    {
        public static string Compile(ImageInfo source, List<ImageInfo> tiles, List<ImageInfo> library, Dictionary<string, string> matches)
        {
            string fileName = $"mosaic-{source.File.Name}";
            string fullName = $"{source.File.Directory}\\{fileName}";
            using (Image<Rgba32> mosaic = new Image<Rgba32>(source.Width, source.Height))
            {
                matches.Keys.ToList().ForEach(key =>
                {
                    ImageInfo sourceTile = tiles.Where(tile => tile.Id == key).FirstOrDefault();
                    ImageInfo targetTile = library.Where(tile => tile.Id == matches[key]).FirstOrDefault();
                    if (sourceTile != null && targetTile != null)
                    {
                        using (Image targetImage = Image.Load(targetTile.File.FullName))
                        {
                            bool resize = targetImage.Width > sourceTile.Width && targetImage.Height > sourceTile.Height;
                            if (resize)
                            {
                                targetImage.Mutate(x => x.Resize(sourceTile.Width, sourceTile.Height));
                            }

                            Point point = new Point(0, 0);
                            string[] position = sourceTile.Id.Split("x");
                            if (position.Length == 2)
                            {
                                int x = int.Parse(position[0]);
                                int y = int.Parse(position[1]);
                                point = new Point(x, y);
                            }

                            mosaic.Mutate(x => x.DrawImage(targetImage, point, 1));
                        }
                    }
                });

                mosaic.Save(fullName);
            }

            return fullName;
        }
    }
}