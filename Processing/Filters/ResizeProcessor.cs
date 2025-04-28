using ImagoCrafter.Core;

namespace ImagoCrafter.Processing.Filters;

public class ResizeProcessor : IImageProcessor
{
    private int _targetWidth;
    private int _targetHeight;

    public ResizeProcessor(int width, int height)
    {
        _targetWidth = width;
        _targetHeight = height;
    }

    public Image Process(Image input)
    {
        var output = new Image(_targetWidth, _targetHeight, input.Channels);
        float xRatio = input.Width / (float)_targetWidth;
        float yRatio = input.Height / (float)_targetHeight;

        Parallel.For(0, _targetHeight, y =>
        {
            for (int x = 0; x < _targetWidth; x++)
            {
                float srcX = x * xRatio;
                float srcY = y * yRatio;
                
                int x1 = (int)srcX;
                int y1 = (int)srcY;
                int x2 = Math.Min(x1 + 1, input.Width - 1);
                int y2 = Math.Min(y1 + 1, input.Height - 1);
                
                float xDiff = srcX - x1;
                float yDiff = srcY - y1;

                for (int c = 0; c < input.Channels; c++)
                {
                    float topLeft = input.GetPixelComponentF(x1, y1, c);
                    float topRight = input.GetPixelComponentF(x2, y1, c);
                    float bottomLeft = input.GetPixelComponentF(x1, y2, c);
                    float bottomRight = input.GetPixelComponentF(x2, y2, c);

                    float top = topLeft + (topRight - topLeft) * xDiff;
                    float bottom = bottomLeft + (bottomRight - bottomLeft) * xDiff;
                    float value = top + (bottom - top) * yDiff;

                    output.SetPixelComponentF(x, y, c, value);
                }
            }
        });

        return output;
    }

    public void Configure(Dictionary<string, object> parameters)
    {
        if (parameters.TryGetValue("width", out var w) && w is int width)
            _targetWidth = width;
        if (parameters.TryGetValue("height", out var h) && h is int height)
            _targetHeight = height;
    }
}