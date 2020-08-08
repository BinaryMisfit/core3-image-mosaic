namespace ImageMosaic
{
    using SixLabors.ImageSharp;
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
                    "SourceFile",
                    "Full path of the source image")
                .ExistingOnly(),
                new Argument<DirectoryInfo>(
                    "LibraryDir",
                    "Path to directory containing the images to use for replacement")
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
                    description: "The directory to use to save temporary files"),
                new Option<bool>(
                    "--unique",
                    description: "Use unique images for replacements")
            };
            commandLine.Description = "Generates a mosaic using a source image and selection of smaller images";
            commandLine.Handler = CommandHandler.Create<
                FileInfo,
                DirectoryInfo,
                int,
                int,
                DirectoryInfo,
                bool>((
                    sourceFile,
                    libraryDir,
                    columns,
                    rows,
                    workingDir,
                    unique) =>
            {
                Console.WriteLine("Processing files");
                ImageInfo sourceImage = FileParser.ParseFile(sourceFile);
                List<ImageInfo> libraryImages = FileParser.ParseDirectory(libraryDir);
                ImageInfo removeSource = libraryImages.Where(image => image.Id == sourceImage.Id).FirstOrDefault();
                if (removeSource != null)
                {
                    libraryImages.Remove(removeSource);
                }

                Console.WriteLine("Files processed");
                Console.WriteLine("Generating tiles");
                Dictionary<string, Image> tiles = TileImage.Tile(sourceImage.File, columns, rows, workingDir);
                List<ImageInfo> sourceTiles = null;
                if (tiles != null && tiles.Count > 0)
                {
                    sourceTiles = new List<ImageInfo>();
                    tiles.Keys.ToList().ForEach(key =>
                    {
                        ImageInfo image = new ImageInfo(key, sourceImage.Format, tiles[key]);
                        sourceTiles.Add(image);
                    });

                    tiles.Clear();
                }

                if (sourceTiles == null)
                {
                    throw new InvalidOperationException("Unable to generate tiles");
                }

                Console.WriteLine("Tiles generated");
                Console.WriteLine("Finding unique matches");
                Dictionary<string, string> matches = new Dictionary<string, string>();
                List<ImageInfo> matchLibrary = new List<ImageInfo>(libraryImages);
                sourceTiles.ForEach(tile =>
                {
                    string matchId = ImageCompare.FindMatch(tile, matchLibrary);
                    matches.Add(tile.Id, matchId);
                    if (unique)
                    {
                        ImageInfo removeMatch = matchLibrary.Where(image => image.Id == matchId).FirstOrDefault();
                        if (removeMatch != null)
                        {
                            matchLibrary.Remove(removeMatch);
                        }
                    }
                });

                Console.WriteLine("Completed matching");
                Console.WriteLine("Composing new image");
                Mosaic.Compile(sourceImage, sourceTiles, libraryImages, matches);
                Console.WriteLine("Image created and saved");
            });
            return commandLine.InvokeAsync(args).Result;
        }
    }
}