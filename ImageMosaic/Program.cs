namespace ImageMosaic
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.IO;
    using System.Linq;

    public class Program
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
                Console.WriteLine("Validating image");
                IImageFile image = new ImageFile(sourceImage);
                if (!image.IsImageFile)
                {
                    Console.WriteLine($"{image.File.FullName} is not a valid image file");
                    Environment.Exit(255);
                }

                Console.WriteLine("Image validated");
                Console.WriteLine("Loading tile library");
                TileLibrary tileLibrary = new TileLibrary(imagePath);
                if (!tileLibrary.HasImages())
                {
                    Console.WriteLine($"{imagePath.FullName} does not contain any images");
                    Environment.Exit(255);
                }

                Console.WriteLine("Tile library loaded");
                SourceImageFile sourceImageFile = new SourceImageFile(image);
                List<TileImage> sourceTiles = new List<TileImage>();
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
                Console.WriteLine("Performing calculations on extracted images");
                sourceTiles.ForEach(tile =>
                {
                    ImageCalculate calculate = new ImageCalculate(tile.Image);
                    tile.RGB = calculate.RGB;
                    tile.XYZ = calculate.XYZ;
                });
                Console.WriteLine("Calculations complete");
                Console.WriteLine("Performing calculations on tile library");
                List<TileImage> invalidImages = new List<TileImage>();
                tileLibrary.Images.ForEach(image =>
                {
                    if (image.Identifier != sourceImage.FullName)
                    {
                        ImageCalculate calculate = new ImageCalculate(image.Image);
                        image.RGB = calculate.RGB;
                        image.XYZ = calculate.XYZ;
                        image.CIE = calculate.CIE;
                    }
                    else
                    {
                        invalidImages.Add(image);
                    }
                });

                if (invalidImages.Count > 0)
                {
                    Console.WriteLine("Removing Invalid Images");
                    tileLibrary.Remove(invalidImages);
                }

                Console.WriteLine("Calculations complete");
            });
            return commandLine.InvokeAsync(args).Result;
        }
    }
}