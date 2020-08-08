namespace ImageMosaic
{
    using System;

    public class CalculateDeltaE
    {
        public CalculateDeltaE(TileImage source, TileImage target)
        {
            Source = source;
            Target = target;
            Calculate();
        }

        public double Distance { get; private set; }
        private TileImage Source { get; }
        private TileImage Target { get; }

        private void Calculate()
        {
            ImageCIE sourceCIE = Source.CIE;
            ImageCIE targetCIE = Target.CIE;
            double deltaE = Math.Pow(sourceCIE.L - targetCIE.L, 2);
            deltaE += Math.Pow(sourceCIE.A - sourceCIE.A, 2);
            deltaE += Math.Pow(sourceCIE.B - sourceCIE.B, 2);
            Distance = deltaE;
        }
    }
}