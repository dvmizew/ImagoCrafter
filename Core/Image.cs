namespace ImagoCrafter.Core;

public class Image
{
    public int Width { get; }
    public int Height { get; }
    public int Channels { get; }
    public byte[] Data { get; }

    public Image(int width, int height, int channels = 3)
    {
        Width = width;
        Height = height;
        Channels = channels;
        Data = new byte[width * height * channels];
    }

    public Image(byte[] data, int width, int height, int channels = 3)
    {
        Data = data;
        Width = width;
        Height = height;
        Channels = channels;
    }

    public byte GetPixelComponent(int x, int y, int channel)
    {
        int index = (y * Width + x) * Channels + channel;
        return Data[index];
    }

    public void SetPixelComponent(int x, int y, int channel, byte value)
    {
        int index = (y * Width + x) * Channels + channel;
        Data[index] = value;
    }
}