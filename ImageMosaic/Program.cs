namespace ImageMosaic
{
    using System;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.IO;

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
                SourceImageFile sourceImageFile = new SourceImageFile(sourceImage);
                if (!sourceImageFile.IsImageFile())
                {
                    Console.WriteLine($"{sourceImageFile.FileName} is not a valid image file");
                    Environment.Exit(255);
                }

                Console.WriteLine($"Splitting image into {columns}x{rows} ({columns * rows})");
                if (workingDir != null && workingDir.Exists)
                {
                    Console.WriteLine($"Working directory set to {workingDir.FullName}");
                }

                sourceImageFile.SplitImage(columns, rows, workingDir);
                if (sourceImageFile.SplitImages == null || sourceImageFile.SplitImages.Count != (columns * rows))
                {
                    throw new InvalidDataException();
                }

                Console.WriteLine("Splitting completed");
            });
            return commandLine.InvokeAsync(args).Result;
        }
    }
}