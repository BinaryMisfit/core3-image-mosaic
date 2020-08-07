namespace ImageMosaic
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.IO;
    using System.Linq;

    internal class Program
    {
        private static int Main(string[] args)
        {
            RootCommand commandLine = new RootCommand
            {
                new Argument<FileInfo>(
                    "SourceImage",
                    "Full path of the source image")
                .ExistingOnly(),
                new Argument<DirectoryInfo>(
                    "ImagePath",
                    "Path to folder containing the images to use for replacement")
                .ExistingOnly(),
                new Option<int>(
                    "--columns",
                    getDefaultValue: () => 20,
                    description: "The number of columns for the overlay grid"
                    ),
                new Option<int>(
                    "--rows",
                    getDefaultValue: () => 20,
                    description: "The number of rows for the overlay grid"
                    ),
                new Option<DirectoryInfo>(
                    "--workingDir",
                    description: "The folder to use to save temporary files")
            };
            commandLine.Description = "Generates a mosaic using a source image and selection of smaller images";
            commandLine.Handler = CommandHandler.Create<
                FileInfo,
                DirectoryInfo,
                int,
                int,
                DirectoryInfo>((
                    sourceImage,
                    imagePath,
                    columns,
                    rows,
                    workingDir) =>
            {
                IImageFile image = new ImageFile(sourceImage);
                List<TileImage> sourceTiles = new List<TileImage>();
                if (!image.IsImageFile)
                {
                    Console.WriteLine($"{image.File.Name} is not a valid image file");
                    Environment.Exit(255);
                }

                SourceImageFile sourceImageFile = new SourceImageFile(image);
                Console.WriteLine($"Splitting image into {columns}x{rows} blocks ({columns * rows})");
                if (workingDir != null && workingDir.Exists)
                {
                    Console.WriteLine($"Working directory set to {workingDir.FullName}");
                }

                sourceImageFile.SplitImage(columns, rows, workingDir);
                if (sourceImageFile.SplitImages == null || sourceImageFile.SplitImages.Count != (columns * rows))
                {
                    throw new InvalidDataException();
                }

                Console.WriteLine($"Extracting {columns * rows} images");
                sourceImageFile.SplitImages.Keys.ToList().ForEach(key =>
                {
                    TileImage tile = new TileImage(key, sourceImageFile.SplitImages[key]);
                    sourceTiles.Add(tile);
                });

                sourceImageFile.SplitImages.Clear();
                Console.WriteLine("Split complete");
                Console.WriteLine($"Calculating average RGB for extracted images");
                sourceTiles.ForEach(tile =>
                {
                    tile.AverageColor = new AverageColor(tile.Image).Calculate();
                });
                Console.WriteLine($"Calculation complete");
            });
            return commandLine.InvokeAsync(args).Result;
        }
    }
}