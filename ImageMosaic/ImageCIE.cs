namespace ImageMosaic
{
    public class ImageCIE
    {
        public ImageCIE(double l, double a, double b)
        {
            L = l;
            A = a;
            B = b;
        }

        public double A { get; }
        public double B { get; }
        public double L { get; }
    }
}