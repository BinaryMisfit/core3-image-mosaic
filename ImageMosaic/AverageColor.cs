namespace ImageMosaic
{
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using System;

    // Average RGB calculation by Manohar Vanga
    // Source: https://sighack.com/post/averaging-rgb-colors-the-right-way
    internal class AverageColor
    {
        public AverageColor(Image image)
        {
            Image = image ?? throw new MissingFieldException();
        }

        public Image Image { get; }

        public Color Calculate()
        {
            using (Image<Rgba32> pixelImage = Image.CloneAs<Rgba32>())
            {
                int redCount = 0;
                int greenCount = 0;
                int blueCount = 0;
                int totalCount = 0;
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
                byte red = Convert.ToByte(redCount);
                byte green = Convert.ToByte(greenCount);
                byte blue = Convert.ToByte(blueCount);
                return Color.FromRgb(red, green, blue);
            }
        }
    }
}