namespace ImagoCrafter.Core;

public class ImageLoader
{
    public static Image Load(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLower();
        return extension switch
        {
            ".bmp" => LoadBmp(filePath),
            _ => LoadRaw(filePath)
        };
    }

    private static Image LoadBmp(string filePath)
    {
        using var fileStream = File.OpenRead(filePath);
        using var reader = new BinaryReader(fileStream);

        // verify BMP signature
        if (reader.ReadChar() != 'B' || reader.ReadChar() != 'M')
            throw new InvalidDataException("Not a valid BMP file");

        // skip file size and reserved fields
        reader.BaseStream.Position = 0x0A;
        int pixelDataOffset = reader.ReadInt32();
        
        // read DIB header size to identify header type
        int dibHeaderSize = reader.ReadInt32();
        
        // read width and height
        int width = reader.ReadInt32();
        int height = Math.Abs(reader.ReadInt32()); // handle both top-down and bottom-up BMPs
        bool topDown = reader.BaseStream.Position - 4 >= 0 && reader.ReadInt32() < 0;
        
        // read color planes and bits per pixel
        reader.BaseStream.Position = 0x1A;
        short bitsPerPixel = reader.ReadInt16();
        int channels = bitsPerPixel / 8;
        
        // read compression method
        int compression = reader.ReadInt32();
        
        if (compression != 0) // BI_RGB = 0 (no compression)
            throw new NotSupportedException("Only uncompressed BMP files are supported");
            
        if (channels != 3 && channels != 4)
            throw new NotSupportedException("Only 24-bit and 32-bit BMP files are supported");

        // move to pixel data
        reader.BaseStream.Position = pixelDataOffset;
        
        // row stride 
        // BMP rows are padded to 4-byte boundaries
        int stride = ((width * channels + 3) / 4) * 4;
        byte[] data = new byte[width * height * 3]; // always convert to RGB

        // read pixel data 
        // BMP is stored in BGR format
        for (int y = 0; y < height; y++)
        {
            int rowY = topDown ? y : (height - 1 - y);
            
            for (int x = 0; x < width; x++)
            {
                int destIndex = (rowY * width + x) * 3;
                
                byte b = reader.ReadByte();
                byte g = reader.ReadByte();
                byte r = reader.ReadByte();
                
                if (channels == 4)
                    reader.ReadByte(); // skip alpha channel
                
                data[destIndex] = r;
                data[destIndex + 1] = g;
                data[destIndex + 2] = b;
            }
            
            // skip padding bytes at the end of each row
            int paddingBytes = stride - (width * channels);
            if (paddingBytes > 0)
                reader.BaseStream.Position += paddingBytes;
        }

        return new Image(data, width, height, 3);
    }

    private static Image LoadRaw(string filePath)
    {
        using var fileStream = File.OpenRead(filePath);
        using var binaryReader = new BinaryReader(fileStream);

        int width = binaryReader.ReadInt32();
        int height = binaryReader.ReadInt32();
        int channels = binaryReader.ReadInt32();

        byte[] data = binaryReader.ReadBytes(width * height * channels);
        return new Image(data, width, height, channels);
    }

    public static void Save(Image image, string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLower();
        if (extension == ".bmp")
            SaveBmp(image, filePath);
        else
            SaveRaw(image, filePath);
    }

    private static void SaveBmp(Image image, string filePath)
    {
        using var fileStream = File.Create(filePath);
        using var writer = new BinaryWriter(fileStream);

        int width = image.Width;
        int height = image.Height;
        int channels = 3; // Always save as 24-bit
        int bitsPerPixel = channels * 8;
        
        // Calculate row stride (padded to 4-byte boundary)
        int stride = ((width * channels + 3) / 4) * 4;
        int paddingSize = stride - (width * channels);
        
        // Calculate data size and file size
        int dataSize = stride * height;
        int headerSize = 54; // BMP header (14) + DIB header (40)
        int fileSize = headerSize + dataSize;

        // BMP Header (14 bytes)
        writer.Write((byte)'B');               // Magic number
        writer.Write((byte)'M');               // Magic number
        writer.Write(fileSize);                // File size
        writer.Write((short)0);                // Reserved 
        writer.Write((short)0);                // Reserved
        writer.Write(headerSize);              // Offset to pixel data
        
        // DIB Header (40 bytes - BITMAPINFOHEADER)
        writer.Write(40);                      // DIB header size (BITMAPINFOHEADER)
        writer.Write(width);                   // Width
        writer.Write(height);                  // Height (positive = bottom-up)
        writer.Write((short)1);                // Color planes
        writer.Write((short)bitsPerPixel);     // Bits per pixel
        writer.Write(0);                       // Compression method (BI_RGB = 0)
        writer.Write(dataSize);                // Image data size
        writer.Write(2835);                    // Horizontal resolution (72 DPI ≈ 2835 pixels/meter)
        writer.Write(2835);                    // Vertical resolution
        writer.Write(0);                       // Colors in palette
        writer.Write(0);                       // Important colors

        // Write pixel data (bottom-up, BGR format with padding)
        byte[] padding = new byte[paddingSize];
        
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                int srcIndex = (y * width + x) * image.Channels;
                
                // Write BGR (reverse of RGB)
                if (image.Channels >= 3)
                {
                    writer.Write(image.Data[srcIndex + 2]); // B
                    writer.Write(image.Data[srcIndex + 1]); // G
                    writer.Write(image.Data[srcIndex]);     // R
                }
                else if (image.Channels == 1)
                {
                    // Grayscale to BGR
                    byte gray = image.Data[srcIndex];
                    writer.Write(gray); // B
                    writer.Write(gray); // G
                    writer.Write(gray); // R
                }
            }
            
            // Write padding bytes at the end of each row
            if (paddingSize > 0)
                writer.Write(padding);
        }
    }

    private static void SaveRaw(Image image, string filePath)
    {
        using var fileStream = File.Create(filePath);
        using var binaryWriter = new BinaryWriter(fileStream);

        binaryWriter.Write(image.Width);
        binaryWriter.Write(image.Height);
        binaryWriter.Write(image.Channels);
        binaryWriter.Write(image.Data);
    }
}