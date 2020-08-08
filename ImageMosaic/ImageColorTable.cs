namespace ImageMosaic
{
    public class ImageColorTable
    {
        public ColorCie Cie { get; set; }
        public ColorRgb Rgb { get; set; }
        public ColorXyz Xyz { get; set; }

        public class ColorCie
        {
            public ColorCie(double l, double a, double b)
            {
                L = l;
                A = a;
                B = b;
            }

            public double A { get; }
            public double B { get; }
            public double L { get; }
        }

        public class ColorRgb
        {
            public ColorRgb(double r, double g, double b)
            {
                R = r;
                G = g;
                B = b;
            }

            public double B { get; }
            public double G { get; }
            public double R { get; }
        }

        public class ColorXyz
        {
            public ColorXyz(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public double X { get; }
            public double Y { get; }
            public double Z { get; }
        }
    }
}