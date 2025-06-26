namespace ImagoCrafter.Core;

public class Image
{
    public int Width { get; }
    public int Height { get; }
    public int Channels { get; }
    public byte[] Data { get; }

    public Image(int width, int height, int channels = 3)
    {
        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive");
        if (height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive");
        if (channels <= 0)
            throw new ArgumentOutOfRangeException(nameof(channels), "Channels must be positive");

        Width = width;
        Height = height;
        Channels = channels;
        
        try
        {
            Data = new byte[width * height * channels];
        }
        catch (OutOfMemoryException ex)
        {
            throw new InvalidOperationException($"Insufficient memory to allocate image of size {width}x{height}x{channels}", ex);
        }
    }

    public Image(byte[] data, int width, int height, int channels = 3)
    {
        ArgumentNullException.ThrowIfNull(data);
        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive");
        if (height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive");
        if (channels <= 0)
            throw new ArgumentOutOfRangeException(nameof(channels), "Channels must be positive");

        int expectedLength = width * height * channels;
        if (data.Length < expectedLength)
            throw new ArgumentException($"Data array length {data.Length} is insufficient for dimensions {width}x{height}x{channels} (expected {expectedLength})");

        Data = data;
        Width = width;
        Height = height;
        Channels = channels;
    }

    public byte GetPixelComponent(int x, int y, int channel)
    {
        if (x < 0 || x >= Width)
            throw new ArgumentOutOfRangeException(nameof(x), $"X coordinate {x} is out of range [0, {Width})");
        if (y < 0 || y >= Height)
            throw new ArgumentOutOfRangeException(nameof(y), $"Y coordinate {y} is out of range [0, {Height})");
        if (channel < 0 || channel >= Channels)
            throw new ArgumentOutOfRangeException(nameof(channel), $"Channel {channel} is out of range [0, {Channels})");

        int index = (y * Width + x) * Channels + channel;
        return Data[index];
    }

    public void SetPixelComponent(int x, int y, int channel, byte value)
    {
        if (x < 0 || x >= Width)
            throw new ArgumentOutOfRangeException(nameof(x), $"X coordinate {x} is out of range [0, {Width})");
        if (y < 0 || y >= Height)
            throw new ArgumentOutOfRangeException(nameof(y), $"Y coordinate {y} is out of range [0, {Height})");
        if (channel < 0 || channel >= Channels)
            throw new ArgumentOutOfRangeException(nameof(channel), $"Channel {channel} is out of range [0, {Channels})");

        int index = (y * Width + x) * Channels + channel;
        Data[index] = value;
    }

    public float GetPixelComponentF(int x, int y, int channel)
    {
        return GetPixelComponent(x, y, channel) / 255.0f;
    }

    public void SetPixelComponentF(int x, int y, int channel, float value)
    {
        SetPixelComponent(x, y, channel, (byte)(value * 255.0f));
    }

    public float GetPixelComponentSafe(int x, int y, int channel)
    {
        x = Math.Clamp(x, 0, Width - 1);
        y = Math.Clamp(y, 0, Height - 1);
        return GetPixelComponentF(x, y, channel);
    }
}