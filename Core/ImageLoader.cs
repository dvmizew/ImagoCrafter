namespace ImagoCrafter.Core;

using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

public class ImageLoader
{
    public static Image Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Image file not found: {filePath}");
        }

        using var imageSharp = SixLabors.ImageSharp.Image.Load<Rgb24>(filePath);
        
        // EXIF orientation correction
        imageSharp.Mutate(x => x.AutoOrient());
        
        return ConvertFromImageSharp(imageSharp);
    }

    public static void Save(Image image, string filePath)
    {
        using var imageSharp = ConvertToImageSharp(image);
        
        // Remove EXIF data when saving since we've already applied orientation corrections
        imageSharp.Metadata.ExifProfile = null;
        
        imageSharp.Save(filePath);
    }

    private static Image ConvertFromImageSharp(Image<Rgb24> imageSharp)
    {
        int width = imageSharp.Width;
        int height = imageSharp.Height;
        byte[] data = new byte[width * height * 3];

        // copy pixel data from ImageSharp format to our linear array format
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Rgb24 pixel = imageSharp[x, y];
                int destIndex = (y * width + x) * 3;
                
                data[destIndex] = pixel.R;
                data[destIndex + 1] = pixel.G;
                data[destIndex + 2] = pixel.B;
            }
        }

        return new Image(data, width, height, 3);
    }
    private static Image<Rgb24> ConvertToImageSharp(Image image)
    {
        var imageSharp = new Image<Rgb24>(image.Width, image.Height);

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
            }
        }

        return imageSharp;
    }
}