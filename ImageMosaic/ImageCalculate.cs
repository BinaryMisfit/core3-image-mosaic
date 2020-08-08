namespace ImageMosaic
{
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using System;

    public class ImageCalculate
    {
        public ImageCalculate(Image image)
        {
            Image = image ?? throw new MissingFieldException();
            ColorTable = new ImageColorTable();
            ColorTable.Rgb = CalculateRgb(image);
            ColorTable.Xyz = CalculateXyz(ColorTable.Rgb);
            ColorTable.Cie = CalculateCie(ColorTable.Xyz);
        }

        public ImageColorTable ColorTable { get; private set; }
        public Image Image { get; }

        private ImageColorTable.ColorCie CalculateCie(ImageColorTable.ColorXyz colorXyz)
        {
            double l = (116 * colorXyz.Y) - 16;
            double a = 500 * (colorXyz.X - colorXyz.Y);
            double b = 200 * (colorXyz.Y - colorXyz.Z);
            return new ImageColorTable.ColorCie(l, a, b);
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

            return calcValue * 100;
        }

        private ImageColorTable.ColorRgb CalculateRgb(Image image)
        {
            using (Image<Rgba32> pixelImage = image.CloneAs<Rgba32>())
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
                return new ImageColorTable.ColorRgb(red, green, blue);
            }
        }

        private ImageColorTable.ColorXyz CalculateXyz(ImageColorTable.ColorRgb colorRgb)
        {
            double red = CalculateIlluminant(colorRgb.R);
            double green = CalculateIlluminant(colorRgb.G);
            double blue = CalculateIlluminant(colorRgb.B);
            double x = red * 0.4124 + green * 0.3576 + blue * 0.1805;
            double y = red * 0.2126 + green * 0.7152 + blue * 0.0722;
            double z = red * 0.0193 + green * 0.1192 + blue * 0.9505;
            return new ImageColorTable.ColorXyz(x, y, z);
        }
    }
}