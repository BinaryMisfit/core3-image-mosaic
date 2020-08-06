namespace ImageMosaic
{
    using SixLabors.ImageSharp;

    internal class AverageColor
    {
        public AverageColor(Image image)
        {
            Image = image;
        }

        public Image Image { get; }
    }
}