namespace ImageMosaic
{
    public class ImageRGB
    {
        public ImageRGB(double red, double green, double blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public double Blue { get; }
        public double Green { get; }
        public double Red { get; }
    }
}