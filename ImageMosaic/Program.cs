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
                .ExistingOnly()
            };
            commandLine.Description = "Generates a mosaic using a source image and selection of smaller images";
            commandLine.Handler = CommandHandler.Create<FileInfo, DirectoryInfo>((sourceImage, imagePath) =>
            {
                SourceImageFile sourceImageFile = new SourceImageFile(sourceImage);
                if (!sourceImageFile.IsImageFile())
                {
                    Console.WriteLine($"{sourceImageFile.FileName} is not a valid image file");
                    Environment.Exit(255);
                }
            });
            return commandLine.InvokeAsync(args).Result;
        }
    }
}