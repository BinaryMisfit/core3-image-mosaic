namespace ImageMosaic
{
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using System;

    public class ImageCalculate
    {
        private const double reference = 100.000;

        public ImageCalculate(Image image)
        {
            Image = image ?? throw new MissingFieldException();
            CalculateRGB();
            CalculateXYZ();
            CalculateCIE();
        }

        public ImageCIE CIE { get; private set; }
        public Image Image { get; }
        public ImageRGB RGB { get; private set; }
        public ImageXYZ XYZ { get; private set; }

        private void CalculateCIE()
        {
            double x = CalculateReference(XYZ.X);
            double y = CalculateReference(XYZ.Y);
            double z = CalculateReference(XYZ.Z);
            double l = (116 * y) - 16;
            double a = 500 * (x - y);
            double b = 200 * (y - z);
            CIE = new ImageCIE(l, a, b);
        }

        private double CalculateIlluminant(double value)
        {
            double calcValue;
            if (value > 0.04045)
            {
                calcValue = (value + 0.055) / 1.055;
                calcValue = Math.Pow(calcValue, 2.4);
            }
            else
            {
                calcValue = value / 12.92;
            }

            return calcValue;
        }

        private double CalculateReference(double value)
        {
            double calcValue = value / reference;
            if (calcValue > 0.008856)
            {
                calcValue = Math.Pow(calcValue, 1 / 3);
            }
            else
            {
                calcValue = (7.787 * calcValue) + (16 / 116);
            }

            return calcValue;
        }

        private void CalculateRGB()
        {
            using (Image<Rgba32> pixelImage = Image.CloneAs<Rgba32>())
            {
                double redCount = 0;
                double greenCount = 0;
                double blueCount = 0;
                double totalCount = 0;
                for (int y = 0; y < pixelImage.Height; y++)
                {
                    Span<Rgba32> pixelRowSpan = pixelImage.GetPixelRowSpan(y);
                    for (int x = 0; x < pixelImage.Width; x++)
                    {
                        redCount += pixelRowSpan[x].R;
                        greenCount += pixelRowSpan[x].G;
                        blueCount += pixelRowSpan[x].B;
                        totalCount++;
                    }
                }

                redCount /= totalCount;
                greenCount /= totalCount;
                blueCount /= totalCount;
                double red = redCount / 255;
                double green = greenCount / 255;
                double blue = blueCount / 255;
                RGB = new ImageRGB(red, green, blue);
            }
        }

        private void CalculateXYZ()
        {
            double red = CalculateIlluminant(RGB.Red);
            double green = CalculateIlluminant(RGB.Green);
            double blue = CalculateIlluminant(RGB.Blue);
            double x = red * 0.4124 + green * 0.3576 + blue * 0.1805;
            double y = red * 0.2126 + green * 0.7152 + blue * 0.0722;
            double z = red * 0.0193 + green * 0.1192 + blue * 0.9505;
            XYZ = new ImageXYZ(x, y, z);
        }
    }
}