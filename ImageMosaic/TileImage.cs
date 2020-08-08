namespace ImageMosaic
{
    using SixLabors.ImageSharp;
    using System;

    public class TileImage
    {
        public TileImage(string identifier, Image image)
        {
            Image = image ?? throw new MissingFieldException();
            Identifier = identifier;
        }

        public ImageCIE CIE { get; set; }
        public string Identifier { get; }
        public Image Image { get; }
        public ImageRGB RGB { get; set; }
        public ImageXYZ XYZ { get; set; }
    }
}