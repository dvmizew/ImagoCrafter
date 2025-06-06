using ImagoCrafter.Core;
using ImagoCrafter.Processing.Kernels;
using ImagoCrafter.Processing.Convolution;

namespace ImagoCrafter.Processing.Filters;

public class ResizeProcessor : IImageProcessor
{
    private int _targetWidth;
    private int _targetHeight;
    private float _sharpenStrength = 0.5f;

    public ResizeProcessor(int width, int height)
    {
        _targetWidth = width;
        _targetHeight = height;
    }
    
    public Image Process(Image input)
    {
        var output = new Image(_targetWidth, _targetHeight, input.Channels);

        if (input.Width <= 0 || input.Height <= 0 || _targetWidth <= 0 || _targetHeight <= 0)
        {
            return output; // return empty image if input is invalid
        }

        float widthRatio = _targetWidth / (float)input.Width;
        float heightRatio = _targetHeight / (float)input.Height;
        float upscaleRatio = Math.Max(widthRatio, heightRatio);
        bool isUpscaling = upscaleRatio > 1.0f;

        var xCoords = new int[_targetWidth][];
        var xWeights = new float[_targetWidth];
        var yCoords = new int[_targetHeight][];
        var yWeights = new float[_targetHeight];

        float xRatio = input.Width / (float)_targetWidth;
        float yRatio = input.Height / (float)_targetHeight;

        for (int x = 0; x < _targetWidth; x++)
        {
            float srcX = x * xRatio;
            int x1 = (int)srcX;
            int x2 = Math.Min(x1 + 1, input.Width - 1);
            xCoords[x] = [x1, x2];
            xWeights[x] = srcX - x1;
        }

        for (int y = 0; y < _targetHeight; y++)
        {
            float srcY = y * yRatio;
            int y1 = (int)srcY;
            int y2 = Math.Min(y1 + 1, input.Height - 1);
            yCoords[y] = [y1, y2];
            yWeights[y] = srcY - y1;
        }

        Parallel.For(0, input.Channels, c =>
        {
            for (int y = 0; y < _targetHeight; y++)
            {
                int y1 = yCoords[y][0];
                int y2 = yCoords[y][1];
                float yWeight = yWeights[y];

                float[] row1 = new float[_targetWidth];
                float[] row2 = new float[_targetWidth];

                for (int x = 0; x < _targetWidth; x++)
                {
                    int x1 = xCoords[x][0];
                    int x2 = xCoords[x][1];
                    float xWeight = xWeights[x];

                    row1[x] = input.GetPixelComponentF(x1, y1, c) * (1 - xWeight) +
                             input.GetPixelComponentF(x2, y1, c) * xWeight;
                    row2[x] = input.GetPixelComponentF(x1, y2, c) * (1 - xWeight) +
                             input.GetPixelComponentF(x2, y2, c) * xWeight;
                }

                for (int x = 0; x < _targetWidth; x++)
                {
                    float value = row1[x] * (1 - yWeight) + row2[x] * yWeight;
                    output.SetPixelComponentF(x, y, c, value);
                }
            }
        });

        // Apply adaptive sharpening if upscaling
        if (isUpscaling)
        {
            float adaptiveStrength = _sharpenStrength * (upscaleRatio - 1.0f);
            float adaptiveRadius = 0.5f + (upscaleRatio - 1.0f) * 0.5f; // Increase radius with scale
            float adaptiveThreshold = 0.01f; // Small threshold to prevent noise enhancement
            
            var sharpenProcessor = new UnsharpMaskProcessor(adaptiveStrength, adaptiveRadius, adaptiveThreshold);
            output = sharpenProcessor.Process(output);
        }

        return output;
    }

    public void Configure(Dictionary<string, object> parameters)
    {
        if (parameters.TryGetValue("width", out var w) && w is int width)
            _targetWidth = width;
        if (parameters.TryGetValue("height", out var h) && h is int height)
            _targetHeight = height;
        if (parameters.TryGetValue("sharpenStrength", out var s) && s is float strength)
            _sharpenStrength = strength;
    }
}