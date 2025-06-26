namespace ImagoCrafter.Core;

using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

public class ImageLoader
{
    public static Image Load(string filePath, IProgressReporter? progressReporter = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Image file not found: {filePath}");
        }

        progressReporter?.Report(0, "Starting image load...");

        try
        {
            progressReporter?.Report(10, "Reading image file...");
            using var imageSharp = SixLabors.ImageSharp.Image.Load<Rgb24>(filePath);
            
            progressReporter?.Report(30, "Validating image...");
            // Validate image dimensions
            if (imageSharp.Width <= 0 || imageSharp.Height <= 0)
            {
                throw new InvalidOperationException($"Invalid image dimensions: {imageSharp.Width}x{imageSharp.Height}");
            }
            
            progressReporter?.Report(50, "Applying orientation correction...");
            // EXIF orientation correction
            imageSharp.Mutate(x => x.AutoOrient());
            
            progressReporter?.Report(70, "Converting image format...");
            var result = ConvertFromImageSharp(imageSharp, progressReporter);
            
            progressReporter?.Report(100, "Image loaded successfully");
            return result;
        }
        catch (UnknownImageFormatException ex)
        {
            throw new NotSupportedException($"Unsupported image format in file: {filePath}", ex);
        }
        catch (InvalidImageContentException ex)
        {
            throw new InvalidDataException($"Corrupted or invalid image data in file: {filePath}", ex);
        }
        catch (OutOfMemoryException ex)
        {
            throw new InvalidOperationException($"Insufficient memory to load image: {filePath}. The image may be too large.", ex);
        }
        catch (Exception ex) when (!(ex is ArgumentException || ex is FileNotFoundException || 
                                    ex is NotSupportedException || ex is InvalidDataException || 
                                    ex is InvalidOperationException))
        {
            throw new InvalidOperationException($"Failed to load image: {filePath}", ex);
        }
    }

    public static void Save(Image image, string filePath, IProgressReporter? progressReporter = null)
    {
        ArgumentNullException.ThrowIfNull(image);

        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        }

        progressReporter?.Report(0, "Starting image save...");

        // Validate output directory exists
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            try
            {
                progressReporter?.Report(10, "Creating output directory...");
                Directory.CreateDirectory(directory);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create output directory: {directory}", ex);
            }
        }

        try
        {
            progressReporter?.Report(30, "Converting image format...");
            using var imageSharp = ConvertToImageSharp(image, progressReporter);
            
            progressReporter?.Report(70, "Removing metadata...");
            // Remove EXIF data when saving since we've already applied orientation corrections
            imageSharp.Metadata.ExifProfile = null;
            
            progressReporter?.Report(80, "Writing image file...");
            imageSharp.Save(filePath);
            
            progressReporter?.Report(100, "Image saved successfully");
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException($"Access denied when saving to: {filePath}", ex);
        }
        catch (DirectoryNotFoundException ex)
        {
            throw new DirectoryNotFoundException($"Directory not found for path: {filePath}", ex);
        }
        catch (IOException ex)
        {
            throw new IOException($"I/O error occurred while saving image to: {filePath}", ex);
        }
        catch (OutOfMemoryException ex)
        {
            throw new InvalidOperationException($"Insufficient memory to save image: {filePath}", ex);
        }
        catch (Exception ex) when (!(ex is ArgumentException || ex is UnauthorizedAccessException || 
                                    ex is DirectoryNotFoundException || ex is IOException || 
                                    ex is InvalidOperationException))
        {
            throw new InvalidOperationException($"Failed to save image: {filePath}", ex);
        }
    }

    private static Image ConvertFromImageSharp(Image<Rgb24> imageSharp, IProgressReporter? progressReporter = null)
    {
        ArgumentNullException.ThrowIfNull(imageSharp);

        int width = imageSharp.Width;
        int height = imageSharp.Height;
        
        try
        {
            byte[] data = new byte[width * height * 3];

            // copy pixel data from ImageSharp format to our linear array format
            int totalPixels = width * height;
            int pixelsProcessed = 0;
            int lastReportedPercentage = 70;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Rgb24 pixel = imageSharp[x, y];
                    int destIndex = (y * width + x) * 3;
                    
                    data[destIndex] = pixel.R;
                    data[destIndex + 1] = pixel.G;
                    data[destIndex + 2] = pixel.B;
                    
                    pixelsProcessed++;
                }

                // Report progress every few rows to avoid excessive updates
                if (progressReporter != null && y % Math.Max(1, height / 20) == 0)
                {
                    int conversionProgress = (int)((double)pixelsProcessed / totalPixels * 30); // 30% of total progress
                    int currentPercentage = lastReportedPercentage + conversionProgress;
                    progressReporter.Report(Math.Min(currentPercentage, 100), "Converting pixels...");
                }
            }

            return new Image(data, width, height, 3);
        }
        catch (OutOfMemoryException ex)
        {
            throw new InvalidOperationException($"Insufficient memory to convert image of size {width}x{height}", ex);
        }
    }
    private static Image<Rgb24> ConvertToImageSharp(Image image, IProgressReporter? progressReporter = null)
    {
        ArgumentNullException.ThrowIfNull(image);

        if (image.Width <= 0 || image.Height <= 0)
        {
            throw new ArgumentException($"Invalid image dimensions: {image.Width}x{image.Height}");
        }

        if (image.Data == null)
        {
            throw new ArgumentException("Image data cannot be null");
        }

        int expectedDataLength = image.Width * image.Height * image.Channels;
        if (image.Data.Length < expectedDataLength)
        {
            throw new ArgumentException($"Image data length {image.Data.Length} is insufficient for dimensions {image.Width}x{image.Height}x{image.Channels}");
        }

        try
        {
            var imageSharp = new Image<Rgb24>(image.Width, image.Height);
            
            int totalPixels = image.Width * image.Height;
            int pixelsProcessed = 0;
            int lastReportedPercentage = 30;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int srcIndex = (y * image.Width + x) * image.Channels;
                    
                    // handle images with different channel counts
                    byte r = image.Channels >= 1 ? image.Data[srcIndex] : (byte)0;
                    byte g = image.Channels >= 2 ? image.Data[srcIndex + 1] : r;
                    byte b = image.Channels >= 3 ? image.Data[srcIndex + 2] : r;
                    
                    imageSharp[x, y] = new Rgb24(r, g, b);
                    
                    pixelsProcessed++;
                }

                // Report progress every few rows to avoid excessive updates
                if (progressReporter != null && y % Math.Max(1, image.Height / 20) == 0)
                {
                    int conversionProgress = (int)((double)pixelsProcessed / totalPixels * 40); // 40% of total progress
                    int currentPercentage = lastReportedPercentage + conversionProgress;
                    progressReporter.Report(Math.Min(currentPercentage, 70), "Converting pixels...");
                }
            }

            return imageSharp;
        }
        catch (OutOfMemoryException ex)
        {
            throw new InvalidOperationException($"Insufficient memory to create ImageSharp image of size {image.Width}x{image.Height}", ex);
        }
    }
}