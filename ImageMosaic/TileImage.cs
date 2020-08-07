namespace ImageMosaic
{
    using SixLabors.ImageSharp;
    using System;

    internal class TileImage
    {
        public TileImage(string identifier, Image image)
        {
            Image = image ?? throw new MissingFieldException();
            Identifier = identifier;
        }

        public Color AverageColor { get; set; }

        public string Identifier { get; }

        public Image Image { get; }
    }
}