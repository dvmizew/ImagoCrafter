namespace ImagoCrafter.Core;

public class Image
{
    private byte[] _data;
    private int _width;
    private int _height;
    private int _channels;

    public int Width => _width;
    public int Height => _height;
    public int Channels => _channels;
    public byte[] Data => _data;

    public Image(int width, int height, int channels = 3)
    {
        _width = width;
        _height = height;
        _channels = channels;
        _data = new byte[width * height * channels];
    }

    public Image(byte[] data, int width, int height, int channels = 3)
    {
        _data = data;
        _width = width;
        _height = height;
        _channels = channels;
    }

    public byte GetPixelComponent(int x, int y, int channel)
    {
        int index = (y * _width + x) * _channels + channel;
        return _data[index];
    }

    public void SetPixelComponent(int x, int y, int channel, byte value)
    {
        int index = (y * _width + x) * _channels + channel;
        _data[index] = value;
    }
}