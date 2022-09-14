using System;

namespace ImageMosaic
{
    public static class DeltaE
    {
        public static double Calculate(ImageColorTable.ColorCie source, ImageColorTable.ColorCie target)
        {
            var deltaE = Math.Pow(source.L - target.L, 2);
            deltaE += Math.Pow(source.A - target.A, 2);
            deltaE += Math.Pow(source.B - target.B, 2);
            return Math.Sqrt(deltaE);
        }
    }
}
