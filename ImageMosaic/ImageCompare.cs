namespace ImageMosaic
{
    using System.Collections.Generic;

    public static class ImageCompare
    {
        public static string FindMatch(ImageInfo image, List<ImageInfo> images)
        {
            string matchId = image.Id;
            double matchDistance = 0;
            images.ForEach(targetImage =>
            {
                double distance = DeltaE.Calculate(image.ColorTable.Cie, targetImage.ColorTable.Cie);
                if (matchDistance == 0)
                {
                    matchId = targetImage.Id;
                    matchDistance = distance;
                }

                if (distance < matchDistance)
                {
                    matchId = targetImage.Id;
                    matchDistance = distance;
                }
            });

            return matchId;
        }
    }
}