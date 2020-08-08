using System.Collections.Generic;

namespace ImageMosaic
{
    public class ImageCompare
    {
        public ImageCompare(List<TileImage> source, List<TileImage> target)
        {
            Source = source;
            Target = target;
        }

        private List<TileImage> Source { get; }
        private List<TileImage> Target { get; }

        public void FindMatch()
        {
            string matchId;
            double matchDistance = 0;
            Source.ForEach(sourceImage =>
            {
                Target.ForEach(targetImage =>
                {
                    CalculateDeltaE deltaE = new CalculateDeltaE(sourceImage, targetImage);
                    double distance = deltaE.Distance;
                    if (matchDistance == 0)
                    {
                        matchId = targetImage.Identifier;
                        matchDistance = distance;
                    }

                    if (distance < matchDistance)
                    {
                        matchId = targetImage.Identifier;
                        matchDistance = distance;
                    }
                });
            });
        }
    }
}